using System;

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace Urls
{
    public record AbsoluteUrl
    (
        string Scheme,
        string Host,
        int? Port,
        RelativeUrl RelativeUrl,
        UserInfo UserInfo
    )
    {
        public static AbsoluteUrl Empty { get; } = new AbsoluteUrl("", "");

        public AbsoluteUrl
            (
        string scheme,
        string host,
        int? port = null,
        RelativeUrl? requestUri = null) : this(scheme, host, port, requestUri ?? RelativeUrl.Empty, new("", ""))
        {

        }

        public AbsoluteUrl(AbsoluteUrl
            absoluteUrl)
        {
            if (absoluteUrl == null) throw new ArgumentNullException(nameof(absoluteUrl));
            Scheme = absoluteUrl.Scheme;
            Host = absoluteUrl.Host;
            Port = absoluteUrl.Port;
            RelativeUrl = new RelativeUrl(absoluteUrl.RelativeUrl);
            UserInfo = absoluteUrl.UserInfo;
        }

#pragma warning disable CA1054 // URI-like parameters should not be strings
        public AbsoluteUrl(string uriString) : this(new Uri(uriString).ToAbsoluteUrl())
#pragma warning restore CA1054 // URI-like parameters should not be strings
        {
        }

        public override string ToString()
        =>
        $"{Scheme}://" + UserInfo +
        $"{Host}" +
        (Port.HasValue ? $":{Port.Value}" : "") +
        RelativeUrl;

        public static implicit operator Uri(AbsoluteUrl absoluteUrl) =>
            absoluteUrl == null ? Empty :
            new Uri(absoluteUrl.ToString());

        public static explicit operator AbsoluteUrl(Uri uri) => uri.ToAbsoluteUrl();
    };
}

