using System;
using System.Collections.Immutable;
using System.Net;

namespace Urls
{
    public record QueryParameter
    {
        private string? fieldValue;

        public static ImmutableList<QueryParameter> EmptyList { get; } = ImmutableList<QueryParameter>.Empty;

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
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            fieldValue = WebUtility.UrlDecode(value);
        }

        public override string ToString()
            => $"{FieldName}{(Value != null ? "=" : "")}{WebUtility.UrlEncode(Value)}";
    }
}

