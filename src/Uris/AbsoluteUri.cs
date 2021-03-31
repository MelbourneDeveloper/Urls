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

