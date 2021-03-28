using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;

#pragma warning disable CA1055 // URI-like return values should not be strings

namespace Uris
{
    public static class UriExtensions
    {
        public static AbsoluteRequestUri ToAbsoluteRequestUri(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var userInfoTokens = uri.UserInfo.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            return new AbsoluteRequestUri(uri.Scheme, uri.Host, uri.Port,
                new RelativeRequestUri(
                    new RequestUriPath(
                        ImmutableList.Create(uri.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                        ),
                    Query.Empty,
#if NET45 || NETSTANDARD2_0
                    uri.Fragment.Replace("#", "")
#else
                    uri.Fragment.Replace("#", "", StringComparison.OrdinalIgnoreCase)
#endif
                    ),
                   uri.UserInfo != null ?
                   new UserInfo(userInfoTokens.First(),
                   userInfoTokens.Length > 1 ? userInfoTokens[1] : "") : null);
        }

        public static AbsoluteRequestUri With(this AbsoluteRequestUri absoluteRequestUri, RelativeRequestUri relativeRequestUri)
        =>
        absoluteRequestUri == null ? throw new ArgumentNullException(nameof(absoluteRequestUri)) :
        new AbsoluteRequestUri(absoluteRequestUri.Scheme, absoluteRequestUri.Host, absoluteRequestUri.Port, relativeRequestUri, absoluteRequestUri.UserInfo);

        public static RelativeRequestUri WithFragment(this RelativeRequestUri uri, string fragment)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        new(uri.Path, uri.Query, fragment);

        public static RelativeRequestUri WithQueryString(this RelativeRequestUri uri, string fieldName, string value)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        new(uri.Path, fieldName.ToQueryParameter(value).ToQuery(), uri.Fragment);

        public static AbsoluteRequestUri WithQueryString(this AbsoluteRequestUri uri, string fieldName, string value)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        new(uri.Scheme, uri.Host, uri.Port, new RelativeRequestUri(RequestUriPath.Empty).WithQueryString(fieldName, value));

        public static QueryParameter ToQueryParameter(this string fieldName, string value) => new(fieldName, value);

        public static Query ToQuery(this QueryParameter queryParameter) => new(Elements: ImmutableList.Create(queryParameter));

        public static string ToUriString(this RelativeRequestUri relativeRequestUri)
        =>
        relativeRequestUri == null ? throw new ArgumentNullException(nameof(relativeRequestUri)) :
        (relativeRequestUri.Path.Elements.Count > 0 ? $"/{string.Join("/", relativeRequestUri.Path.Elements)}" : "") +
        (relativeRequestUri.Query.Elements.Count > 0 ? $"?{string.Join("&", relativeRequestUri.Query.Elements.Select(e => $"{e.FieldName}={WebUtility.UrlEncode(e.Value)}"))}" : "") +
        (!string.IsNullOrEmpty(relativeRequestUri.Fragment) ? $"#{relativeRequestUri.Fragment}" : "");

        public static string ToUriString(this AbsoluteRequestUri absoluteRequestUri)
        =>
        absoluteRequestUri == null ? throw new ArgumentNullException(nameof(absoluteRequestUri)) :
        $"{absoluteRequestUri.Scheme}://" +
        $"{(absoluteRequestUri.UserInfo != null ? $"{absoluteRequestUri.UserInfo.Username}:{absoluteRequestUri.UserInfo.Password}@" : "")}" +
        $"{absoluteRequestUri.Host}" +
        (absoluteRequestUri.Port.HasValue ? $":{absoluteRequestUri.Port.Value}" : "") +
        absoluteRequestUri.RequestUri.ToUriString();
    }
}
