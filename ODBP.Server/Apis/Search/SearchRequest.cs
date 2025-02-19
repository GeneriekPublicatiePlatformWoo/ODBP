using System.Text.Json.Serialization;

namespace ODBP.Apis.Search
{
    public record SearchRequest
    {
        public string? Query { get; init; }
        public int? Page { get; init; }
        public int? PageSize { get; init; }
        public Sort? Sort { get; init; }
        public DateTimeOffset? RegistratiedatumVanaf { get; init; }
        public DateTimeOffset? RegistratiedatumTot { get; init; }
        public DateTimeOffset? LaatstGewijzigdDatumVanaf { get; init; }
        public DateTimeOffset? LaatstGewijzigdDatumTot { get; init; }
        public ResultType? ResultType { get; init; }
        public string[] Publishers { get; init; } = [];
        public string[] InformatieCategorieen { get; init; } = [];
    }

    [JsonConverter(typeof(JsonStringEnumConverter<Sort>))]
    public enum Sort
    {
        relevance,
        chronological
    }
}
