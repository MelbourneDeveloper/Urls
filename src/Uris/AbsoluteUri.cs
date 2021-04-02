using System;

#pragma warning disable CA2225 // Operator overloads have named alternates

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
        public static AbsoluteUri Empty { get; } = new AbsoluteUri("", "");

        public AbsoluteUri
            (
        string scheme,
        string host,
        int? port = null,
        RelativeUri? requestUri = null) : this(scheme, host, port, requestUri ?? RelativeUri.Empty, new("", ""))
        {

        }

        public AbsoluteUri(AbsoluteUri absoluteUri)
        {
            if (absoluteUri == null) throw new ArgumentNullException(nameof(absoluteUri));
            Scheme = absoluteUri.Scheme;
            Host = absoluteUri.Host;
            Port = absoluteUri.Port;
            RelativeUri = new RelativeUri(absoluteUri.RelativeUri);
            UserInfo = absoluteUri.UserInfo;
        }

#pragma warning disable CA1054 // URI-like parameters should not be strings
        public AbsoluteUri(string uriString) : this(new Uri(uriString).ToAbsoluteUri())
#pragma warning restore CA1054 // URI-like parameters should not be strings
        {
        }

        public override string ToString()
        =>
        $"{Scheme}://" + UserInfo +
        $"{Host}" +
        (Port.HasValue ? $":{Port.Value}" : "") +
        RelativeUri;

        public static implicit operator Uri(AbsoluteUri absoluteUri) =>
            absoluteUri == null ? Empty :
            new Uri(absoluteUri.ToString());

        public static explicit operator AbsoluteUri(Uri uri) => uri.ToAbsoluteUri();
    };
}

