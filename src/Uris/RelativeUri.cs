using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Urls
{
    /// <summary>
    /// Represents a Url without the Host, Scheme, Port or UserInfo 
    /// </summary>
    public record RelativeUrl

    (
         ImmutableList<string> Path,
         ImmutableList<QueryParameter> QueryParameters,
         string Fragment
    )
    {
        public RelativeUrl(
        IReadOnlyList<string>? path = null,
        IReadOnlyList<QueryParameter>? query = null,
        string? fragment = null) : this(path != null ? path.ToImmutableList() :
        ImmutableList<string>.Empty, query?.ToImmutableList() ?? ImmutableList<QueryParameter>.Empty, fragment ?? "")
        {
        }

        public RelativeUrl(
        string? path = null,
        IReadOnlyList<QueryParameter>? query = null,
        string? fragment = null) : this(path != null ? path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToImmutableList() :
        ImmutableList<string>.Empty, query?.ToImmutableList() ?? ImmutableList<QueryParameter>.Empty, fragment ?? "")
        {
        }

        public RelativeUrl(RelativeUrl relativeUrl)
        {
            if (relativeUrl == null) throw new ArgumentNullException(nameof(relativeUrl));
            Path = relativeUrl.Path;
            QueryParameters = relativeUrl.QueryParameters;
            Fragment = relativeUrl.Fragment;
        }

        public static RelativeUrl Empty { get; } = new(ImmutableList<string>.Empty, ImmutableList<QueryParameter>.Empty);

        public override string ToString()
        =>
        (Path.Count > 0 ? $"/{string.Join("/", Path)}" : "") +
        (QueryParameters.Count > 0 ? $"?{string.Join("&", QueryParameters.Select(e => e.ToString()))}" : "") +
        (!string.IsNullOrEmpty(Fragment) ? $"#{Fragment}" : "");
    }
}

