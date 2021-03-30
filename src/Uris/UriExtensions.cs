using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable IDE0057 // Use range operator

namespace Uris
{
    public static class UriExtensions
    {
        public static AbsoluteUri ToAbsoluteUri(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var userInfoTokens = uri.UserInfo?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            var queryParametersList = new List<QueryParameter>();

            var queryParameterTokens = (uri.Query ?? "").Substring(1).Split(new char[] { '&' });

            queryParametersList.AddRange(queryParameterTokens.Select(keyValueString => keyValueString.Split(new char[] { '=' })).Select(keyAndValue
                => new QueryParameter(keyAndValue.First(), keyAndValue.Length > 1 ? keyAndValue[1] : null)));

            return new AbsoluteUri(uri.Scheme, uri.Host, uri.Port,
                new RelativeUri(
                        ImmutableList.Create(uri.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                        ,
                    queryParametersList.Count == 0 ? ImmutableList<QueryParameter>.Empty : queryParametersList.ToImmutableList(),
                    uri.Fragment.Substring(1)
                    ),
                   userInfoTokens != null ? new UserInfo(userInfoTokens.First(),
                       userInfoTokens.Length > 1 ? userInfoTokens[1] : "") : new("", ""));
        }

        public static AbsoluteUri RelativeUri(this AbsoluteUri absoluteRequestUri, RelativeUri relativeRequestUri)
        =>
        absoluteRequestUri == null ? throw new ArgumentNullException(nameof(absoluteRequestUri)) :
        new AbsoluteUri(absoluteRequestUri.Scheme, absoluteRequestUri.Host, absoluteRequestUri.Port, relativeRequestUri, absoluteRequestUri.UserInfo);

        public static RelativeUri Fragment(this RelativeUri uri, string fragment)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        new(uri.Path, uri.QueryParameters, fragment);

        public static RelativeUri AddQueryString(this RelativeUri uri, string fieldName, string value)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        uri with { QueryParameters = uri.QueryParameters.Add(new QueryParameter(fieldName, value)) };

        public static AbsoluteUri AddQueryParameter(this AbsoluteUri uri, string fieldName, string value)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        uri with { RelativeUri = uri.RelativeUri with { QueryParameters = uri.RelativeUri.QueryParameters.Add(new QueryParameter(fieldName, value)) } };

        public static QueryParameter ToQueryParameter(this string fieldName, string value) => new(fieldName, value);

        public static ImmutableList<QueryParameter> ToQuery(this QueryParameter queryParameter) => ImmutableList.Create(queryParameter);


        public static RelativeUri QueryParamers<T>(this RelativeUri relativeUri, T item)
        =>
        relativeUri == null ? throw new ArgumentNullException(nameof(relativeUri)) :
            relativeUri with
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

        public static AbsoluteUri Credentials(this AbsoluteUri uri, string username, string password)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        uri with { UserInfo = new(username, password) };

        public static AbsoluteUri Fragment(this AbsoluteUri uri, string fragment)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        uri with { RelativeUri = uri.RelativeUri with { Fragment = fragment } };

        public static AbsoluteUri Path(this AbsoluteUri uri, IReadOnlyList<string> pathSegments)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        uri with { RelativeUri = uri.RelativeUri with { Path = pathSegments.ToImmutableList() } };

        public static AbsoluteUri Path(this AbsoluteUri uri, params string[] pathSegments)
        => Path(uri, pathSegments.ToList());

        public static AbsoluteUri ToHttpUriFromHost(this string host, int? port = null)
        =>
        host == null ? throw new ArgumentNullException(nameof(host)) :
                new AbsoluteUri("http", host, port);

        public static AbsoluteUri ToHttpsUriFromHost(this string host, int? port = null)
        =>
        host == null ? throw new ArgumentNullException(nameof(host)) :
                new AbsoluteUri("https", host, port);

        public static AbsoluteUri ToAbsoluteUri(this string uriString)
        => new Uri(uriString, UriKind.Absolute).ToAbsoluteUri();

    }
}
