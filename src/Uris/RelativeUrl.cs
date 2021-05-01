using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Urls
{
    /// <summary>
    /// Represents a Url without the specifics of the server address
    /// </summary>
    public record RelativeUrl

    (
         ImmutableList<string> Path,
         ImmutableList<QueryParameter> QueryParameters
    )
    {
        #region Fields
        private string fragment = "";
        #endregion

        #region Public Properties
        public string Fragment
        {
            get => fragment;
            init
            {
                fragment = value ?? "";
            }
        }
        #endregion

        #region Constructors
        public RelativeUrl(
        IReadOnlyList<string>? path = null,
        IReadOnlyList<QueryParameter>? query = null,
        string fragment = "") : this(
            path != null ? path.ToImmutableList() : ImmutableList<string>.Empty, query?.ToImmutableList() ?? QueryParameter.EmptyList) => this.fragment = fragment ?? "";

        public RelativeUrl(
#pragma warning disable CA1054 // URI-like parameters should not be strings
        string relativeUrlString) : this(relativeUrlString.ToRelativeUrl())
#pragma warning restore CA1054 // URI-like parameters should not be strings
        {
        }

        public RelativeUrl(RelativeUrl relativeUrl)
        {
            relativeUrl ??= Empty;

            Path = relativeUrl.Path;
            QueryParameters = relativeUrl.QueryParameters;
            fragment = relativeUrl.Fragment ?? "";
        }

        public static RelativeUrl Empty { get; } = new(ImmutableList<string>.Empty, QueryParameter.EmptyList);
        #endregion

        #region Public Methods
        public override string ToString()
        =>
        (Path.Count > 0 ? $"/{string.Join("/", Path)}" : "") +
        (QueryParameters.Count > 0 ? $"?{string.Join("&", QueryParameters.Select(e => e.ToString()))}" : "") +
        (!string.IsNullOrEmpty(Fragment) ? $"#{Fragment}" : "");

        public virtual bool Equals(RelativeUrl? other) =>
            other != null &&
            string.CompareOrdinal(other.Fragment, Fragment) == 0 &&
            other.Path.SequenceEqual(Path) &&
            other.QueryParameters.SequenceEqual(QueryParameters);

        // Optional: warning generated if not supplied when Equals(R?) is user-defined.
        public override int GetHashCode() => ToString().GetHashCode(
#if NET5_0
            StringComparison.InvariantCultureIgnoreCase
#endif
            );
        #endregion
    }
}

