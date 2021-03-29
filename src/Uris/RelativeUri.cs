using System.Collections.Immutable;

namespace Uris
{
    public record RelativeUri
    (
         ImmutableList<string> Path,
         Query Query,
         string Fragment
    )
    {
        public RelativeUri(
        ImmutableList<string>? path = null,
        Query? query = null) : this(path ?? ImmutableList<string>.Empty, query ?? Query.Empty, "")
        {

        }

        public static RelativeUri Empty { get; } = new(ImmutableList<string>.Empty, Query.Empty);

    };
}

