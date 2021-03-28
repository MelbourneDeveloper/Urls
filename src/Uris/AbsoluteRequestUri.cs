namespace Uris
{
    public record AbsoluteRequestUri
    (
        string Scheme,
        string Host,
        int? Port,
        RelativeRequestUri RequestUri,
        UserInfo? UserInfo
    )
    {
        public AbsoluteRequestUri(
        string scheme,
        string host,
        int? port = null,
        RelativeRequestUri? requestUri = null) : this(scheme, host, port, requestUri ?? RelativeRequestUri.Empty, default)
        {

        }
    };
}

