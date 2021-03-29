using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable CA1055 // URI-like return values should not be strings
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0050 // Convert to tuple
#pragma warning disable CA1305 // Specify IFormatProvider

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
            var uriString = new AbsoluteUri(Scheme, Host, Port,
                new RelativeUri(
                        ImmutableList.Create(PathPart1, PathPart2),
                        ImmutableList.Create(
                            new QueryParameter(FieldName1, FieldValue1),
                            new QueryParameter(FieldName2, FieldValue2)
                            )
                    , Fragment),
                    new UserInfo(Username, Password)).ToString();

            var expected = $"{Scheme}://{Username}:{Password}@{Host}:{Port}/{PathPart1}/{PathPart2}?" +
                $"{FieldName1}={FieldValueEncoded1}&{FieldName2}={FieldValueEncoded2}#{Fragment}";

            Assert.AreEqual(
                expected,
                uriString);
        }

        [TestMethod]
        public void TestAbsoluteWithRelative()
        {
            var absolute = new AbsoluteUri(Scheme, Host);

            var relativeRequestUri = new RelativeUri(
                                    ImmutableList.Create(PathPart1, PathPart2),
                                    ImmutableList.Create(
                                        new QueryParameter(FieldName1, FieldValue1),
                                        new QueryParameter(FieldName2, FieldValue2)
                                        )
                                );

            absolute = absolute.With(relativeRequestUri);

            Assert.AreEqual(
                relativeRequestUri.Fragment,
                absolute.RequestUri?.Fragment);
        }

        [TestMethod]
        public void TestRelativeWithFragment()
        {
            var relativeRequestUri = new RelativeUri(
                                    ImmutableList.Create(PathPart1, PathPart2),
                                    ImmutableList.Create(
                                        new QueryParameter(FieldName1, FieldValue1),
                                        new QueryParameter(FieldName2, FieldValue2)
                                        )
                                );

            const string frag = "test";

            relativeRequestUri = relativeRequestUri.WithFragment(frag);

            Assert.AreEqual(
                frag,
                relativeRequestUri.Fragment);
        }

        [TestMethod]
        public void TestWithQueryStringStrings()
        {
            var relativeRequestUri = RelativeUri.Empty.WithQueryString(FieldName1, FieldValue1);

            Assert.AreEqual(
            FieldName1,
            relativeRequestUri.QueryParameters?.First().FieldName
            );

            Assert.AreEqual(
            FieldValue1,
            relativeRequestUri.QueryParameters?.First().Value
            );
        }

        [TestMethod]
        public void TestAbsoluteWithQueryStringStrings()
        {
            var absoluteRequestUri = new AbsoluteUri("https", "test.com");

            absoluteRequestUri = absoluteRequestUri.WithQueryString(FieldName1, FieldValue1);

            Assert.AreEqual(
            FieldName1,
            absoluteRequestUri.RequestUri.QueryParameters.First().FieldName
            );

            Assert.AreEqual(
            FieldValue1,
            absoluteRequestUri.RequestUri.QueryParameters.First().Value
            );
        }

        [TestMethod]
        public void TestMinimalAbsoluteToString()
        => Assert.AreEqual("https://test.com", new AbsoluteUri("https", "test.com").ToString());

        [TestMethod]
        public void TestConstructUri()
        {
            var uriString = new AbsoluteUri(Scheme, Host, Port,
                new RelativeUri(
                        ImmutableList.Create(PathPart1, PathPart2)
                        ,
                        ImmutableList.Create(
                            new QueryParameter(FieldName1, FieldValue1),
                            new QueryParameter(FieldName2, FieldValue2)
                            )
                    , Fragment),
                    new UserInfo(Username, Password)).ToString();

            var uri = new Uri(uriString, UriKind.Absolute);

            Assert.IsNotNull(uri);
            Assert.AreEqual(uri.Scheme, Scheme);
        }

        [TestMethod]
        public void TestWithQueryParams()
        {
            var item = new
            {
                somelongstring = "gvhhvhgfgfdg7676878",
                count = 50,
                message = "This is a sentence"
            };

            var relativeUri = RelativeUri.Empty.WithQueryParamers(item);

            Assert.AreEqual(item.somelongstring, relativeUri.QueryParameters[0].Value);
            Assert.AreEqual(nameof(item.somelongstring), relativeUri.QueryParameters[0].FieldName);
            Assert.AreEqual(item.count.ToString(), relativeUri.QueryParameters[1].Value);
            Assert.AreEqual(nameof(item.count), relativeUri.QueryParameters[1].FieldName);
            Assert.AreEqual(item.message, relativeUri.QueryParameters[2].Value);
            Assert.AreEqual(nameof(item.message), relativeUri.QueryParameters[2].FieldName);
        }

        [TestMethod]
        public void TestFromUri()
        {
            var uriString = new AbsoluteUri(Scheme, Host, Port,
                new RelativeUri(
                        ImmutableList.Create(PathPart1, PathPart2),
                        ImmutableList.Create(
                            new QueryParameter(FieldName1, FieldValue1),
                            new QueryParameter(FieldName2, FieldValue2)
                            )
                    , Fragment),
                    new UserInfo(Username, Password)).ToString();

            var uri = new Uri(uriString, UriKind.Absolute).ToAbsoluteRequestUri();

            Assert.IsNotNull(uri);
            Assert.AreEqual(uri.Scheme, Scheme);
            Assert.AreEqual(uri.RequestUri.Fragment, Fragment);
            Assert.AreEqual(uri.RequestUri.QueryParameters.First().FieldName, FieldName1);
            Assert.AreEqual(uri.RequestUri.QueryParameters.First().Value, FieldValueEncoded1);
            Assert.AreEqual(uri.RequestUri.QueryParameters[1].FieldName, FieldName2);
            Assert.AreEqual(uri.RequestUri.QueryParameters[1].Value, FieldValueEncoded2);
            Assert.AreEqual(Host, uri.Host);
            Assert.AreEqual(Port, uri.Port);
            Assert.AreEqual(PathPart1, uri.RequestUri.Path[0]);
            Assert.AreEqual(PathPart2, uri.RequestUri.Path[1]);
            Assert.AreEqual(Fragment, uri.RequestUri.Fragment);
            Assert.AreEqual(Username, uri.UserInfo?.Username);
            Assert.AreEqual(Password, uri.UserInfo?.Password);
        }

    }


}
