using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.AspNetCore.WebUtilities;

namespace ODBP.Apis.Search;

/// <summary>
/// TIJDELIJK!!! Deze 'mock' kan weg als de functionaliteit in de zoeken component van gpp-woo geimplementeerd is
/// </summary>
/// <param name="acc"></param>
/// <param name="elastic"></param>
public class SearchClientMock(IHttpContextAccessor acc, ElasticsearchClient elastic) : ISearchClient
{
    private const string IndexName = "woo-mock";
    private const string KeywordSuffix = "keyword";
    private static readonly string[] s_fields = ["officieleTitel", "verkorteTitel", "omschrijving", "identificatie"];

    public static IServiceCollection AddToServices(IServiceCollection services, IConfiguration config)
    {
        var settings = new ElasticsearchClientSettings(new Uri(config["ELASTICSEARCH_BASE_URL"]))
            .Authentication(new BasicAuthentication(config["ELASTICSEARCH_USERNAME"], config["ELASTICSEARCH_PASSWORD"]))
            .DisableDirectStreaming();
        var elasticsearch = new ElasticsearchClient(settings);
        services.AddSingleton(elasticsearch);
        services.AddSingleton<ISearchClient, SearchClientMock>();
        return services;
    }

    #region Search
    public async Task<PaginatedSearchResults> Search(SearchRequest request, CancellationToken token)
    {
        var validPage = Math.Max(request.Page ?? 1, 1);
        var validPageSize = Math.Min(Math.Max(request.PageSize ?? 5, 5), 50);
        var from = (validPage - 1) * validPageSize;
        var response = await elastic.SearchAsync<SearchResult>(x => x
            .Index(IndexName)
            .From(from)
            .Size(validPageSize)
            .Query(q => q
                .Bool(b =>
                {
                    var musts = GetMustFilters(request).ToArray();
                    var filters = GetFilters(request).ToArray();

                    if (musts.Length > 0)
                    {
                        b = b.Must(musts);
                    }

                    if (filters.Length > 0)
                    {
                        b.Filter(filters);
                    }
                }))
            .Aggregations(aa => aa
                .Add(nameof(SearchResult.InformatieCategorieen), a => a.MultiTerms(t => t.Terms(r => r.Field(z => z.InformatieCategorieen[0].Uuid.Suffix(KeywordSuffix)), r => r.Field(z => z.InformatieCategorieen[0].Name.Suffix(KeywordSuffix)))))
                .Add(nameof(SearchResult.Publisher), a => a.MultiTerms(t => t.Terms(r => r.Field(z => z.Publisher.Uuid.Suffix(KeywordSuffix)), r => r.Field(z => z.Publisher.Name.Suffix(KeywordSuffix)))))
                .Add(nameof(SearchResult.ResultType), a => a.Terms(t => t.Field(r => r.ResultType.Suffix(KeywordSuffix)))))
            .Sort(GetSort(request))
            , token);

        if (!response.IsSuccess() || response.Aggregations == null)
        {
            throw new Exception();
        }

        var queryDict = acc.HttpContext?.Request.Query.ToDictionary(StringComparer.OrdinalIgnoreCase) ?? [];
        queryDict["pageSize"] = validPageSize.ToString();
        var path = $"/api/zoeken";
        string? previous = null;
        string? next = null;

        if (response.Total > validPage * validPageSize)
        {
            queryDict["page"] = (validPage + 1).ToString();
            next = QueryHelpers.AddQueryString(path, queryDict);
        }

        if (validPage > 1)
        {
            queryDict["page"] = (validPage - 1).ToString();
            previous = QueryHelpers.AddQueryString(path, queryDict);
        }

        return new PaginatedSearchResults
        {
            Count = response.Total,
            Facets = new Facets
            {
                InformatieCategorieen = response.Aggregations.GetMultiTerms(nameof(SearchResult.InformatieCategorieen))?.Buckets.Select(b => new Bucket() { Count = b.DocCount, Name = b.Key.ElementAt(1).ToString(), Uuid = b.Key.ElementAt(0).ToString() }).ToArray() ?? [],
                Organisaties = response.Aggregations.GetMultiTerms(nameof(SearchResult.Publisher))?.Buckets.Select(b => new Bucket() { Count = b.DocCount, Name = b.Key.ElementAt(1).ToString(), Uuid = b.Key.ElementAt(0).ToString() }).ToArray() ?? [],
                ResultTypes = response.Aggregations.GetStringTerms(nameof(SearchResult.ResultType))?.Buckets.Select(b => new ResultTypeBucket() { Count = b.DocCount, Name = Enum.Parse<ResultType>(b.Key.ToString()) }).ToArray() ?? []
            },
            Results = response.Documents,
            Next = next,
            Previous = previous,
        };
    }

    private static Action<SortOptionsDescriptor<SearchResult>>[] GetSort(SearchRequest request) => request.Sort == Sort.Chronological
            ? [
                s => s.Field(f => f.LaatstGewijzigdDatum, f => f.Order(SortOrder.Desc)),
                s =>s.Score(s => s.Order(SortOrder.Desc))
            ]
            : [
                s =>s.Score(s => s.Order(SortOrder.Desc)),
                s => s.Field(f => f.LaatstGewijzigdDatum, f => f.Order(SortOrder.Desc))
            ];

    private static IEnumerable<Action<QueryDescriptor<SearchResult>>> GetMustFilters(SearchRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            yield return f => f.MultiMatch(s => s
                .Fields(s_fields)
                .Query(request.Query)
                .Fuzziness(new(2)));
        }

    }

    private static IEnumerable<Action<QueryDescriptor<SearchResult>>> GetFilters(SearchRequest request)
    {


        var publishers = request.Publishers.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => FieldValue.String(x)).ToArray();
        if (publishers.Length > 0)
        {
            yield return f => f.Terms(t => t
                .Field(r => r.Publisher.Uuid.Suffix(KeywordSuffix))
                .Terms(new TermsQueryField(publishers)));
        }
        var info = request.InformatieCategorieen.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => FieldValue.String(x)).ToArray();
        if (info.Length > 0)
        {
            yield return f => f.Terms(t => t
                .Field(r => r.InformatieCategorieen[0].Uuid.Suffix(KeywordSuffix))
                .Terms(new TermsQueryField(info)));
        }

        if (request.ResultType.HasValue)
        {
            yield return f => f.Term(t => t.Field(r => r.ResultType.Suffix(KeywordSuffix)).Value(request.ResultType.Value.ToString()));
        }
        if (request.RegistratiedatumVanaf.HasValue || request.RegistratiedatumTot.HasValue)
        {
            yield return f => f.Range(range => range.DateRange(dr =>
            {
                dr.Field(r => r.Registratiedatum);
                if (request.RegistratiedatumVanaf.HasValue)
                {
                    dr.Gte(DateMath.Anchored(request.RegistratiedatumVanaf.Value.DateTime));
                }
                if (request.RegistratiedatumTot.HasValue)
                {
                    dr.Lte(DateMath.Anchored(request.RegistratiedatumTot.Value.DateTime));
                }
            }));
        }
        if (request.LaatstGewijzigdDatumVanaf.HasValue || request.LaatstGewijzigdDatumTot.HasValue)
        {
            yield return f => f.Range(range => range.DateRange(dr =>
            {
                dr.Field(r => r.LaatstGewijzigdDatum);
                if (request.LaatstGewijzigdDatumVanaf.HasValue)
                {
                    dr.Gte(DateMath.Anchored(request.LaatstGewijzigdDatumVanaf.Value.DateTime));
                }
                if (request.LaatstGewijzigdDatumTot.HasValue)
                {
                    dr.Lte(DateMath.Anchored(request.LaatstGewijzigdDatumTot.Value.DateTime));
                }
            }));
        }
    }
    #endregion

    #region Seed
    public static async Task Seed(WebApplication app)
    {

        var elasticsearch = app.Services.GetRequiredService<ElasticsearchClient>();
        var headResponse = await elasticsearch.Transport.HeadAsync("/" + IndexName);

        if (headResponse.ApiCallDetails.HttpStatusCode != 404)
        {
            return;
        }

        try
        {
            await elasticsearch.Indices.CreateAsync<SearchResult>(IndexName);

            await foreach (var item in GetRecordsFromFile(default))
            {
                await elasticsearch.IndexAsync(item, x => x.Index(IndexName));
            }
        }
        catch
        {
        }
    }

    private static readonly JsonSerializerOptions s_serializerOptions = new(JsonSerializerDefaults.Web);

    private static async IAsyncEnumerable<SearchResult> GetRecordsFromFile([EnumeratorCancellation] CancellationToken token)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var qualifiedName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains("mock-content.json"));
        if (string.IsNullOrWhiteSpace(qualifiedName)) yield break;
        await using var stream = assembly.GetManifestResourceStream(qualifiedName)!;
        await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<SearchResult>(stream, s_serializerOptions, cancellationToken: token).WithCancellation(token))
        {
            if (item != null) yield return item;
        }
    }
    #endregion

}
