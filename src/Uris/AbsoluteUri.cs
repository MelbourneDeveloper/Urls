namespace Uris
{
    public record AbsoluteUri
    (
        string Scheme,
        string Host,
        int? Port,
        RelativeUri RequestUri,
        UserInfo? UserInfo
    )
    {
        public AbsoluteUri
            (
        string scheme,
        string host,
        int? port = null,
        RelativeUri? requestUri = null) : this(scheme, host, port, requestUri ?? RelativeUri.Empty, default)
        {

        }

        public override string ToString()
        =>
        $"{Scheme}://" +
        $"{(UserInfo != null ? $"{UserInfo.Username}:{UserInfo.Password}@" : "")}" +
        $"{Host}" +
        (Port.HasValue ? $":{Port.Value}" : "") +
        RequestUri;
    };
}

