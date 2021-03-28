using System.Collections.Immutable;

namespace Uris
{
    public record Query
    (
        ImmutableList<QueryParameter> Elements
    )
    {
        public static Query Empty { get; } = new(ImmutableList<QueryParameter>.Empty);
    };
}

