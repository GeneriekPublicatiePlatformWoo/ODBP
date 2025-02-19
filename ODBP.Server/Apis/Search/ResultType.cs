using System.Text.Json.Serialization;

namespace ODBP.Apis.Search;

[JsonConverter(typeof(JsonStringEnumConverter<ResultType>))]
public enum ResultType
{
    publication,
    document
}
