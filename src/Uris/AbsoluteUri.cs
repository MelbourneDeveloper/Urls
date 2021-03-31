namespace Uris
{
    public record AbsoluteUri
    (
        string Scheme,
        string Host,
        int? Port,
        RelativeUri RelativeUri,
        UserInfo UserInfo
    )
    {
        public AbsoluteUri
            (
        string scheme,
        string host,
        int? port = null,
        RelativeUri? requestUri = null) : this(scheme, host, port, requestUri ?? RelativeUri.Empty, new("", ""))
        {

        }

        public override string ToString()
        =>
        $"{Scheme}://" + UserInfo +
        $"{Host}" +
        (Port.HasValue ? $":{Port.Value}" : "") +
        RelativeUri;
    };
}

