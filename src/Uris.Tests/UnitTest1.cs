using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;

#pragma warning disable CA1055 // URI-like return values should not be strings

namespace Uris.UnitTests
{
    [TestClass]
    public class UriTests
    {
        private const string Scheme = "http";
        private const string Host = "host.com";
        private const int Port = 5000;
        private const string PathPart1 = "pathpart1";
        private const string PathPart2 = "pathpart2";
        private const string FieldName1 = "fieldname1";
        private const string FieldName2 = "FieldName2";
        private const string FieldValue1 = "field<>Value1";
        private const string FieldValue2 = "field<>Value2";
        private const string FieldValueEncoded1 = "field%3C%3EValue1";
        private const string FieldValueEncoded2 = "field%3C%3EValue2";
        private const string Fragment = "frag";
        private const string Username = "username";
        private const string Password = "password";

        [TestMethod]
        public void Test()
        {
            var uriString = new AbsoluteRequestUri(Scheme, Host, Port,
                new RelativeRequestUri(
                    new RequestUriPath(
                        ImmutableList.Create(PathPart1, PathPart2)
                        ),
                    new Query(
                        ImmutableList.Create(
                            new QueryParameter(FieldName1, FieldValue1),
                            new QueryParameter(FieldName2, FieldValue2)
                            )
                    ), Fragment),
                    new UserInfo(Username, Password)).ToUriString();

            var expected = $"{Scheme}://{Username}:{Password}@{Host}:{Port}/{PathPart1}/{PathPart2}?" +
                $"{FieldName1}={FieldValueEncoded1}&{FieldName2}={FieldValueEncoded2}#{Fragment}";

            Assert.AreEqual(
                expected,
                uriString);
        }

        [TestMethod]
        public void TestAbsoluteWithRelative()
        {
            var absolute = new AbsoluteRequestUri(Scheme, Host);

            var relativeRequestUri = new RelativeRequestUri(
                                new RequestUriPath(
                                    ImmutableList.Create(PathPart1, PathPart2)
                                    ),
                                new Query(
                                    ImmutableList.Create(
                                        new QueryParameter(FieldName1, FieldValue1),
                                        new QueryParameter(FieldName2, FieldValue2)
                                        )
                                ));

            absolute = absolute.With(relativeRequestUri);

            Assert.AreEqual(
                relativeRequestUri.Fragment,
                absolute.RequestUri?.Fragment);
        }

        [TestMethod]
        public void TestRelativeWithFragment()
        {
            var relativeRequestUri = new RelativeRequestUri(
                                new RequestUriPath(
                                    ImmutableList.Create(PathPart1, PathPart2)
                                    ),
                                new Query(
                                    ImmutableList.Create(
                                        new QueryParameter(FieldName1, FieldValue1),
                                        new QueryParameter(FieldName2, FieldValue2)
                                        )
                                ));

            const string frag = "test";

            relativeRequestUri = relativeRequestUri.WithFragment(frag);

            Assert.AreEqual(
                frag,
                relativeRequestUri.Fragment);
        }

        [TestMethod]
        public void TestWithQueryStringStrings()
        {
            var relativeRequestUri = RelativeRequestUri.Empty.WithQueryString(FieldName1, FieldValue1);

            Assert.AreEqual(
            FieldName1,
            relativeRequestUri.Query?.Elements.First().FieldName
            );

            Assert.AreEqual(
            FieldValue1,
            relativeRequestUri.Query?.Elements.First().Value
            );
        }

        [TestMethod]
        public void TestAbsoluteWithQueryStringStrings()
        {
            var absoluteRequestUri = new AbsoluteRequestUri("https", "test.com");

            absoluteRequestUri = absoluteRequestUri.WithQueryString(FieldName1, FieldValue1);

            Assert.AreEqual(
            FieldName1,
            absoluteRequestUri?.RequestUri?.Query?.Elements.First().FieldName
            );

            Assert.AreEqual(
            FieldValue1,
            absoluteRequestUri?.RequestUri?.Query?.Elements.First().Value
            );
        }

        [TestMethod]
        public void TestMinimalAbsoluteToString()
        => Assert.AreEqual("https://test.com", new AbsoluteRequestUri("https", "test.com").ToUriString());

        [TestMethod]
        public void TestConstructUri()
        {
            var uriString = new AbsoluteRequestUri(Scheme, Host, Port,
                new RelativeRequestUri(
                    new RequestUriPath(
                        ImmutableList.Create(PathPart1, PathPart2)
                        ),
                    new Query(
                        ImmutableList.Create(
                            new QueryParameter(FieldName1, FieldValue1),
                            new QueryParameter(FieldName2, FieldValue2)
                            )
                    ), Fragment),
                    new UserInfo(Username, Password)).ToUriString();

            var uri = new Uri(uriString, UriKind.Absolute);

            Assert.IsNotNull(uri);
            Assert.AreEqual(uri.Scheme, Scheme);
        }


        [TestMethod]
        public void TestFromUri()
        {
            var uriString = new AbsoluteRequestUri(Scheme, Host, Port,
                new RelativeRequestUri(
                    new RequestUriPath(
                        ImmutableList.Create(PathPart1, PathPart2)
                        ),
                    new Query(
                        ImmutableList.Create(
                            new QueryParameter(FieldName1, FieldValue1),
                            new QueryParameter(FieldName2, FieldValue2)
                            )
                    ), Fragment),
                    new UserInfo(Username, Password)).ToUriString();

            var uri = new Uri(uriString, UriKind.Absolute).ToAbsoluteRequestUri();

            Assert.IsNotNull(uri);
            Assert.AreEqual(uri.Scheme, Scheme);
            Assert.AreEqual(uri.RequestUri.Fragment, Fragment);

            //Break it
            Assert.AreEqual(uri.RequestUri.Query.Elements.First().FieldName, FieldName1);
        }

    }

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
#if NET45
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
