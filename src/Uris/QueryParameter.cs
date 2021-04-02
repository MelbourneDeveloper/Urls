using System.Net;

namespace Urls
{
    public record QueryParameter
    (
        string FieldName,
        string? Value
    )
    {
        public override string ToString()
            => $"{FieldName}={WebUtility.UrlEncode(Value)}";
    }
}

