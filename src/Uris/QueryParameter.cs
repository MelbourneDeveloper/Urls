using System.Net;

namespace Urls
{
    public record QueryParameter
    {
        private string? fieldValue;

        public string FieldName { get; init; }
        public string? Value
        {
            get => fieldValue; init
            {
                fieldValue = WebUtility.UrlDecode(value);
            }
        }

        public QueryParameter(string fieldName, string? value)
        {
            FieldName = fieldName;
            fieldValue = WebUtility.UrlDecode(value);
        }

        public override string ToString()
            => $"{FieldName}{(Value != null ? "=" : "")}{WebUtility.UrlEncode(Value)}";
    }
}

