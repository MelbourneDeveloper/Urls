namespace Uris
{
    public record RelativeRequestUri
    (
         RequestUriPath Path,
         Query Query,
         string Fragment
    )
    {
        public RelativeRequestUri(
        RequestUriPath? path = null,
        Query? query = null) : this(path ?? RequestUriPath.Empty, query ?? Query.Empty, "")
        {

        }

        public static RelativeRequestUri Empty { get; } = new(RequestUriPath.Empty, Query.Empty);

    };
}

