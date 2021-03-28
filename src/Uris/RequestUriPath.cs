using System.Collections.Immutable;

namespace Uris
{
    public record RequestUriPath
    (

        ImmutableList<string> Elements
    )
    {
        public static RequestUriPath Empty { get; } = new(ImmutableList<string>.Empty);
    };
}

