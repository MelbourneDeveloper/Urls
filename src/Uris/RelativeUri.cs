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
         Query Query,
         string Fragment
    )
    {
        public RelativeUri(
        IReadOnlyList<string>? path = null,
        Query? query = null) : this(path != null ? path.ToImmutableList() : ImmutableList<string>.Empty, query ?? Query.Empty, "")
        {
        }

        public static RelativeUri Empty { get; } = new(ImmutableList<string>.Empty, Query.Empty);

        public override string ToString()
        =>
        (Path.Count > 0 ? $"/{string.Join("/", Path)}" : "") +
        (Query.Elements.Count > 0 ? $"?{string.Join("&", Query.Elements.Select(e => $"{e.FieldName}={WebUtility.UrlEncode(e.Value)}"))}" : "") +
        (!string.IsNullOrEmpty(Fragment) ? $"#{Fragment}" : "");
    };
}

