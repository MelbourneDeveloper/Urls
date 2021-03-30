using System.Net;

namespace Uris
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

