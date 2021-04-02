using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Uris
{
    /// <summary>
    /// Represents a Uri without the Host, Scheme, Port or UserInfo 
    /// </summary>
    public record RelativeUri
    (
         ImmutableList<string> Path,
         ImmutableList<QueryParameter> QueryParameters,
         string Fragment
    )
    {
        public RelativeUri(
        IReadOnlyList<string>? path = null,
        IReadOnlyList<QueryParameter>? query = null,
        string? fragment = null) : this(path != null ? path.ToImmutableList() :
        ImmutableList<string>.Empty, query?.ToImmutableList() ?? ImmutableList<QueryParameter>.Empty, fragment ?? "")
        {
        }

        public RelativeUri(
        string? path = null,
        IReadOnlyList<QueryParameter>? query = null,
        string? fragment = null) : this(path != null ? path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToImmutableList() :
        ImmutableList<string>.Empty, query?.ToImmutableList() ?? ImmutableList<QueryParameter>.Empty, fragment ?? "")
        {
        }

        public RelativeUri(RelativeUri relativeUri)
        {
            if (relativeUri == null) throw new ArgumentNullException(nameof(relativeUri));
            Path = relativeUri.Path;
            QueryParameters = relativeUri.QueryParameters;
            Fragment = relativeUri.Fragment;
        }

        public static RelativeUri Empty { get; } = new(ImmutableList<string>.Empty, ImmutableList<QueryParameter>.Empty);

        public override string ToString()
        =>
        (Path.Count > 0 ? $"/{string.Join("/", Path)}" : "") +
        (QueryParameters.Count > 0 ? $"?{string.Join("&", QueryParameters.Select(e => e.ToString()))}" : "") +
        (!string.IsNullOrEmpty(Fragment) ? $"#{Fragment}" : "");
    }
}

