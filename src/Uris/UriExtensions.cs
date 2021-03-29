using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable IDE0057 // Use range operator

namespace Uris
{
    public static class UriExtensions
    {
        public static AbsoluteUri ToAbsoluteRequestUri(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var userInfoTokens = uri.UserInfo.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            var queryParametersList = new List<QueryParameter>();
            if (uri.Query.Length <= 0)
            {
                return new AbsoluteUri(uri.Scheme, uri.Host, uri.Port,
                    new RelativeUri(
                            ImmutableList.Create(uri.LocalPath.Split(new char[] { '/' },
                                StringSplitOptions.RemoveEmptyEntries))
                        ,
                        queryParametersList.Count == 0
                            ? Query.Empty
                            : new Query(ImmutableList.Create(queryParametersList.ToArray())),
                        uri.Fragment.Substring(1)
                    ),
                    new UserInfo(userInfoTokens.First(),
                        userInfoTokens.Length > 1 ? userInfoTokens[1] : ""));
            }

            var queryParameterTokens = uri.Query.Substring(1).Split(new char[] { '&' });

            queryParametersList.AddRange(queryParameterTokens.Select(keyValueString => keyValueString.Split(new char[] { '=' })).Select(keyAndValue
                => new QueryParameter(keyAndValue.First(), keyAndValue.Length > 1 ? keyAndValue[1] : null)));

            return new AbsoluteUri(uri.Scheme, uri.Host, uri.Port,
                new RelativeUri(
                        ImmutableList.Create(uri.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                        ,
                    queryParametersList.Count == 0 ? Query.Empty : new Query(ImmutableList.Create(queryParametersList.ToArray())),
                    uri.Fragment.Substring(1)
                    ),
                   new UserInfo(userInfoTokens.First(),
                       userInfoTokens.Length > 1 ? userInfoTokens[1] : ""));
        }

        public static AbsoluteUri With(this AbsoluteUri absoluteRequestUri, RelativeUri relativeRequestUri)
        =>
        absoluteRequestUri == null ? throw new ArgumentNullException(nameof(absoluteRequestUri)) :
        new AbsoluteUri(absoluteRequestUri.Scheme, absoluteRequestUri.Host, absoluteRequestUri.Port, relativeRequestUri, absoluteRequestUri.UserInfo);

        public static RelativeUri WithFragment(this RelativeUri uri, string fragment)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        new(uri.Path, uri.Query, fragment);

        public static RelativeUri WithQueryString(this RelativeUri uri, string fieldName, string value)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        new(uri.Path, fieldName.ToQueryParameter(value).ToQuery(), uri.Fragment);

        public static AbsoluteUri WithQueryString(this AbsoluteUri uri, string fieldName, string value)
        =>
        uri == null ? throw new ArgumentNullException(nameof(uri)) :
        new(uri.Scheme, uri.Host, uri.Port, new RelativeUri().WithQueryString(fieldName, value));

        public static QueryParameter ToQueryParameter(this string fieldName, string value) => new(fieldName, value);

        public static Query ToQuery(this QueryParameter queryParameter) => new(Elements: ImmutableList.Create(queryParameter));
    }
}
