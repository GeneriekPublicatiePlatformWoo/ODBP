using System.Runtime.CompilerServices;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.AspNetCore.WebUtilities;
using ODBP.Apis.Odrc;
using ODBP.Features.Sitemap.SitemapInstances;

namespace ODBP.Apis.Search;

/// <summary>
/// TIJDELIJK!!! Deze 'mock' kan weg als de functionaliteit in de zoeken component van gpp-woo geimplementeerd is
/// </summary>
/// <param name="acc"></param>
/// <param name="elastic"></param>
public class SearchClientMock(IHttpContextAccessor acc, ElasticsearchClient elastic) : ISearchClient
{
    const string ApiVersion = "v1";
    const string ApiRoot = $"/api/{ApiVersion}";
    const string OrganisatiesPath = $"{ApiRoot}/organisaties?isActief=alle";
    const string InformatieCategorieenPath = $"{ApiRoot}/informatiecategorieen";
    const string DocumentenRoot = $"{ApiRoot}/documenten";
    const string DocumentenQueryPath = $"{DocumentenRoot}?publicatiestatus=gepubliceerd&sorteer=creatiedatum";
    private const string IndexName = "woo-mock";
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
                .Add(nameof(SearchResult.InformatieCategorieen), a => a
                    .Nested(n => n.Path(r => r.InformatieCategorieen))
                        .Aggregations(zzz => zzz.Add(nameof(SearchResult.InformatieCategorieen), xxx => xxx.MultiTerms(t => t.Terms(r => r.Field(z => z.InformatieCategorieen[0].Uuid), r => r.Field(z => z.InformatieCategorieen[0].Name))))))
                .Add(nameof(SearchResult.Publisher), a => a
                    .Nested(n => n.Path(r => r.Publisher))
                        .Aggregations(zzz => zzz.Add(nameof(SearchResult.Publisher), xxx => xxx.MultiTerms(t => t.Terms(r => r.Field(z => z.Publisher.Uuid), r => r.Field(z => z.Publisher.Name))))))
                .Add(nameof(SearchResult.ResultType), a => a
                    .Terms(t => t.Field(r => r.ResultType))))
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
                InformatieCategorieen = response.Aggregations.GetNested(nameof(SearchResult.InformatieCategorieen))?.Aggregations.GetMultiTerms(nameof(SearchResult.InformatieCategorieen))?.Buckets.Select(b => new Bucket() { Count = b.DocCount, Name = b.Key.ElementAt(1).ToString(), Uuid = b.Key.ElementAt(0).ToString() }).ToArray() ?? [],
                Organisaties = response.Aggregations.GetNested(nameof(SearchResult.Publisher))?.Aggregations.GetMultiTerms(nameof(SearchResult.Publisher))?.Buckets.Select(b => new Bucket() { Count = b.DocCount, Name = b.Key.ElementAt(1).ToString(), Uuid = b.Key.ElementAt(0).ToString() }).ToArray() ?? [],
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

        var publishers = request.Publishers.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => FieldValue.String(x)).ToArray();
        if (publishers.Length > 0)
        {
            yield return f => f.Nested(n => n
                .Path(r => r.Publisher)
                .Query(q => q
                    .Bool(b => b
                        .Filter(ff => ff
                            .Terms(t => t
                                .Field(r => r.Publisher.Uuid)
                                .Terms(new TermsQueryField(publishers)))))));
        }
        var info = request.InformatieCategorieen.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => FieldValue.String(x)).ToArray();
        if (info.Length > 0)
        {
            yield return f => f.Nested(n => n
                .Path(r => r.InformatieCategorieen)
                .Query(q => q
                    .Bool(b => b
                        .Filter(ff => ff
                            .Terms(t => t
                                .Field(r => r.InformatieCategorieen[0].Uuid)
                                .Terms(new TermsQueryField(info)))))));
        }
    }

    private static IEnumerable<Action<QueryDescriptor<SearchResult>>> GetFilters(SearchRequest request)
    {

        if (request.ResultType.HasValue)
        {
            yield return f => f.Term(t => t.Field(r => r.ResultType).Value(request.ResultType.Value.ToString()));
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
        CancellationToken token = default;
        var elasticsearch = app.Services.GetRequiredService<ElasticsearchClient>();
        using var odrcClient = app.Services.GetRequiredService<IOdrcClientFactory>().Create("Seed elasticsearch mock");
        var organisatiesTask = SitemapController.GetWaardelijstDictionary(odrcClient, OrganisatiesPath, token);
        var informatiecategorieenTask = SitemapController.GetWaardelijstDictionary(odrcClient, InformatieCategorieenPath, token);

        var gepubliceerdePublicaties = await SitemapController.GetGepubliceerdePublicatieDictionary(odrcClient, token);
        var organisaties = await organisatiesTask;
        var informatiecategorieen = await informatiecategorieenTask;
        var exists = (await elasticsearch.Transport.HeadAsync("/" + IndexName));

        if (exists.ApiCallDetails.HttpStatusCode == 404)
        {
            var z = await elasticsearch.Indices.CreateAsync<SearchResult>(IndexName, i => i.Mappings(m => m.Properties(p => p
            .Keyword(p => p.ResultType)
            .Nested(p => p.InformatieCategorieen, zz => zz.Properties(xx => xx.Keyword(yy => yy.InformatieCategorieen[0].Uuid).Keyword(yy => yy.InformatieCategorieen[0].Name)))
            .Nested(p => p.Publisher, zz => zz.Properties(xx => xx.Keyword(yy => yy.Publisher.Uuid).Keyword(yy => yy.Publisher.Name)))
            )), default);

            foreach (var publicatie in gepubliceerdePublicaties.Values)
            {
                var record = new SearchResult
                {
                    InformatieCategorieen = SitemapController.LookupValuesInDictionary(publicatie.InformatieCategorieen, informatiecategorieen).Select(x => new InformatieCategorie { Name = x.Value, Uuid = x.Resource }).ToArray(),
                    LaatstGewijzigdDatum = publicatie.LaatstGewijzigdDatum,
                    OfficieleTitel = publicatie.OfficieleTitel,
                    VerkorteTitel = publicatie.VerkorteTitel,
                    Omschrijving = publicatie.Omschrijving,
                    Publisher = organisaties.TryGetValue(publicatie.Publisher, out var org) ? new Org { Name = org.Value, Uuid = org.Resource } : null,
                    Registratiedatum = publicatie.Registratiedatum,
                    ResultType = ResultType.Publication,
                    Uuid = publicatie.Uuid,
                };
                await elasticsearch.IndexAsync(record, x => x.Index(IndexName));
            }

            await foreach (var item in GetAllPages(odrcClient, DocumentenQueryPath, token))
            {
                var document = item.Deserialize(SitemapPublicatieContext.Default.OdrcDocument);
                if (!gepubliceerdePublicaties.TryGetValue(document!.Publicatie, out var publicatie))
                {
                    continue;
                }
                var record = new SearchResult
                {
                    InformatieCategorieen = SitemapController.LookupValuesInDictionary(publicatie.InformatieCategorieen, informatiecategorieen).Select(x => new InformatieCategorie { Name = x.Value, Uuid = x.Resource }).ToArray(),
                    LaatstGewijzigdDatum = document.LaatstGewijzigdDatum,
                    OfficieleTitel = document.OfficieleTitel,
                    VerkorteTitel = document.VerkorteTitel,
                    Omschrijving = document.Omschrijving,
                    Publisher = organisaties.TryGetValue(publicatie.Publisher, out var org) ? new Org { Name = org.Value, Uuid = org.Resource } : null,
                    Registratiedatum = DateTimeOffset.Parse(document.Creatiedatum),
                    ResultType = ResultType.Document,
                    Uuid = document.Uuid,
                };
                await elasticsearch.IndexAsync(record, x => x.Index(IndexName));
            }
        }
    }

    /// <summary>
    /// Doorloopt alle pagina's van een API-response en retourneert de resultaten.
    /// </summary>
    private static async IAsyncEnumerable<JsonElement> GetAllPages(HttpClient client, string url, [EnumeratorCancellation] CancellationToken token)
    {
        string? next = null;
        using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
        {
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(token);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: token);
            if (doc.RootElement.TryGetProperty("next", out var nextProp) && nextProp.ValueKind == JsonValueKind.String)
            {
                next = nextProp.GetString();
            }
            if (doc.RootElement.TryGetProperty("results", out var resultsProp) && resultsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in resultsProp.EnumerateArray())
                {
                    yield return item;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(next))
        {
            await foreach (var item in GetAllPages(client, next, token))
            {
                yield return item;
            }
        }
    }

    #endregion

}
