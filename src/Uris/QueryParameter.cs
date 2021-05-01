using System;
using System.Collections.Immutable;
using System.Net;

namespace Urls
{
    /// <summary>
    /// Represents a singular Query parameter as part of a Query String
    /// </summary>
    public record QueryParameter
    {
        #region Fields
        private string? fieldValue;
        #endregion

        #region Public Properties
        public static ImmutableList<QueryParameter> EmptyList { get; } = ImmutableList<QueryParameter>.Empty;

        public string FieldName { get; init; }
        public string? Value
        {
            get => fieldValue; init
            {
                fieldValue = WebUtility.UrlDecode(value);
            }
        }
        #endregion

        #region Constructors
        public QueryParameter(string fieldName, string? value)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            fieldValue = WebUtility.UrlDecode(value);
        }
        #endregion

        #region Public Methods
        public override string ToString()
            => $"{FieldName}{(Value != null ? "=" : "")}{WebUtility.UrlEncode(Value)}";
        #endregion
    }
}

