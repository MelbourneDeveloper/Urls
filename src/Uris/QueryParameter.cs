using System.Net;

namespace Urls
{
    public record QueryParameter
    {
        public string FieldName { get; }
        public string? Value { get; }

        public QueryParameter(string fieldName, string? value)
        {
            FieldName = fieldName;
            Value = WebUtility.UrlDecode(value);
        }

        public override string ToString()
            => $"{FieldName}={WebUtility.UrlEncode(Value)}";
    }
}

