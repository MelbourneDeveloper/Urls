using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;

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
        ImmutableList<QueryParameter>? query = null) : this(path != null ? path.ToImmutableList() : ImmutableList<string>.Empty, query ?? ImmutableList<QueryParameter>.Empty, "")
        {
        }

        public static RelativeUri Empty { get; } = new(ImmutableList<string>.Empty, ImmutableList<QueryParameter>.Empty);

        public override string ToString()
        =>
        (Path.Count > 0 ? $"/{string.Join("/", Path)}" : "") +
        (QueryParameters.Count > 0 ? $"?{string.Join("&", QueryParameters.Select(e => $"{e.FieldName}={WebUtility.UrlEncode(e.Value)}"))}" : "") +
        (!string.IsNullOrEmpty(Fragment) ? $"#{Fragment}" : "");
    };
}

