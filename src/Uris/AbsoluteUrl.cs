using System;

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace Urls
{
    /// <summary>
    /// Represents a Url with all information
    /// </summary>
    public record AbsoluteUrl
    (
        string Scheme,
        string Host,
        int? Port,
        RelativeUrl RelativeUrl,
        UserInfo UserInfo
    )
    {
        #region Public Properties
        public static AbsoluteUrl Empty { get; } = new AbsoluteUrl("", "");
        #endregion

        #region Constructors
        public AbsoluteUrl
            (
        string scheme,
        string host,
        int? port = null,
        RelativeUrl? requestUri = null) : this(scheme, host, port, requestUri ?? RelativeUrl.Empty, UserInfo.Empty)
        {

        }

        public AbsoluteUrl(AbsoluteUrl
            absoluteUrl)
        {
            absoluteUrl ??= Empty;

            Scheme = absoluteUrl.Scheme;
            Host = absoluteUrl.Host;
            Port = absoluteUrl.Port;
            RelativeUrl = new RelativeUrl(absoluteUrl.RelativeUrl);
            UserInfo = absoluteUrl.UserInfo;
        }

        //TODO: Parse the string instead of converting to a Uri first
#pragma warning disable CA1054 // URI-like parameters should not be strings
        public AbsoluteUrl(string uriString) : this(new Uri(uriString).ToAbsoluteUrl())
#pragma warning restore CA1054 // URI-like parameters should not be strings
        {
        }
        #endregion

        #region Public Methods
        public override string ToString()
        =>
        $"{Scheme}://" + UserInfo +
        $"{Host}" +
        (Port.HasValue ? $":{Port.Value}" : "") +
        RelativeUrl;
        #endregion

        #region Operators
        public static implicit operator Uri(AbsoluteUrl absoluteUrl) =>
            absoluteUrl == null ? Empty :
            new Uri(absoluteUrl.ToString());

        public static explicit operator AbsoluteUrl(Uri uri) => uri.ToAbsoluteUrl();
        #endregion
    }
}

