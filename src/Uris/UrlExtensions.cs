using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0057 // Use range operator

[assembly: InternalsVisibleTo("Urls.Tests")]

namespace Urls
{
    public static class UrlExtensions
    {
        internal const string ErrorMessageMustBeAbsolute = "The Uri must be an absolute Uri";

        public static Uri ToUri(this AbsoluteUrl url) =>
            url == null ? throw new ArgumentNullException(nameof(url)) :
            new Uri(url.ToString());

        public static AbsoluteUrl ToAbsoluteUrl(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (!uri.IsAbsoluteUri) throw new InvalidOperationException(ErrorMessageMustBeAbsolute);

            var userInfoTokens = uri.UserInfo?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            var relativeUrl = ToRelativeUrl(uri);

            var userInfo = userInfoTokens != null && userInfoTokens.Length > 0 ? new UserInfo(userInfoTokens.First(),
                                   userInfoTokens.Length > 1 ? userInfoTokens[1] : "") : new("", "");

            return new AbsoluteUrl(
                uri.Scheme,
                uri.Host,
                uri.Port,
                relativeUrl,
                   userInfo);
        }

        public static RelativeUrl ToRelativeUrl(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var path = ImmutableList.Create(uri.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));

            var queryParametersList = new List<QueryParameter>();

            var queryParameterTokens = new string[0];
            if (uri.Query != null && uri.Query.Length >= 1)
            {
                queryParameterTokens = uri.Query.Substring(1).Split(new char[] { '&' });
            }

            queryParametersList.AddRange(queryParameterTokens.Select(keyValueString => keyValueString.Split(new char[] { '=' })).Select(keyAndValue
                => new QueryParameter(keyAndValue.First(), keyAndValue.Length > 1 ? keyAndValue[1] : null)));

            var fragment = uri.Fragment != null && uri.Fragment.Length >= 1 ? uri.Fragment.Substring(1) : "";

            return new RelativeUrl(
                    path,
                    queryParametersList.Count == 0 ? ImmutableList<QueryParameter>.Empty : queryParametersList.ToImmutableList(),
                    fragment
                    );
        }

        public static AbsoluteUrl WithRelativeUrl(this AbsoluteUrl absoluteUrl, RelativeUrl relativeUrl)
        =>
        absoluteUrl == null ? throw new ArgumentNullException(nameof(absoluteUrl)) :
        new AbsoluteUrl(absoluteUrl.Scheme, absoluteUrl.Host, absoluteUrl.Port, relativeUrl, absoluteUrl.UserInfo);

        public static RelativeUrl WithFragment(this RelativeUrl relativeUrl, string fragment)
        =>
        relativeUrl == null ? throw new ArgumentNullException(nameof(relativeUrl)) :
        new(relativeUrl.Path, relativeUrl.QueryParameters, fragment);

        public static RelativeUrl AddQueryString(this RelativeUrl relativeUrl, string fieldName, string value)
        =>
        relativeUrl == null ? throw new ArgumentNullException(nameof(relativeUrl)) :
        relativeUrl with { QueryParameters = relativeUrl.QueryParameters.Add(new QueryParameter(fieldName, value)) };

        public static AbsoluteUrl AddQueryParameter(this AbsoluteUrl absoluteUrl, string fieldName, string value)
        =>
        absoluteUrl == null ? throw new ArgumentNullException(nameof(absoluteUrl)) :
        absoluteUrl with { RelativeUrl = absoluteUrl.RelativeUrl with { QueryParameters = absoluteUrl.RelativeUrl.QueryParameters.Add(new QueryParameter(fieldName, value)) } };

        public static QueryParameter ToQueryParameter(this string fieldName, string value) => new(fieldName, value);

        public static RelativeUrl WithQueryParameters<T>(this RelativeUrl relativeUrl, T item)
        =>
        relativeUrl == null ? throw new ArgumentNullException(nameof(relativeUrl)) :
            relativeUrl with
            {
                QueryParameters =
                typeof(T)
                .GetProperties()
                .Select(propertyInfo
                => new QueryParameter(
                    propertyInfo.Name,
                    propertyInfo.GetValue(item)?.ToString())
                ).ToImmutableList()
            };

        public static AbsoluteUrl WithCredentials(this AbsoluteUrl absoluteUrl, string username, string password)
        =>
        absoluteUrl == null ? throw new ArgumentNullException(nameof(absoluteUrl)) :
        absoluteUrl with { UserInfo = new(username, password) };

        public static AbsoluteUrl WithFragment(this AbsoluteUrl absoluteUrl, string fragment)
        =>
        absoluteUrl == null ? throw new ArgumentNullException(nameof(absoluteUrl)) :
        absoluteUrl with { RelativeUrl = absoluteUrl.RelativeUrl with { Fragment = fragment } };

        public static AbsoluteUrl WithPath(this AbsoluteUrl absoluteUrl, IReadOnlyList<string> pathSegments)
        =>
        absoluteUrl == null ? throw new ArgumentNullException(nameof(absoluteUrl)) :
        absoluteUrl with { RelativeUrl = absoluteUrl.RelativeUrl with { Path = pathSegments.ToImmutableList() } };

        public static AbsoluteUrl WithPath(this AbsoluteUrl absoluteUrl, params string[] pathSegments)
        => WithPath(absoluteUrl, pathSegments.ToList());

        public static AbsoluteUrl ToHttpUriFromHost(this string host, int? port = null)
        =>
        host == null ? throw new ArgumentNullException(nameof(host)) :
                new AbsoluteUrl("http", host, port);

        public static AbsoluteUrl ToHttpsUriFromHost(this string host, int? port = null)
        =>
        host == null ? throw new ArgumentNullException(nameof(host)) :
                new AbsoluteUrl("https", host, port);

        public static AbsoluteUrl ToAbsoluteUrl(this string urlString)
        => new Uri(urlString, UriKind.Absolute).ToAbsoluteUrl();

        public static RelativeUrl AppendPath(this RelativeUrl relativeUrl, params string[] args)
            =>
            relativeUrl == null ? throw new ArgumentNullException(nameof(relativeUrl)) :
            relativeUrl with { Path = relativeUrl.Path.AddRange(args) };

        public static AbsoluteUrl AppendPath(this AbsoluteUrl absoluteUrl, params string[] args)
            =>
            absoluteUrl == null ? throw new ArgumentNullException(nameof(absoluteUrl)) :
            absoluteUrl with { RelativeUrl = absoluteUrl.RelativeUrl.AppendPath(args) };

    }
}
