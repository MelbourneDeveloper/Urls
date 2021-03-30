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

        private readonly string expected = $"{Scheme}://{Username}:{Password}@{Host}:{Port}/{PathPart1}/{PathPart2}?" +
            $"{FieldName1}={FieldValueEncoded1}&{FieldName2}={FieldValueEncoded2}#{Fragment}";



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


            Assert.AreEqual(
                expected,
                uriString);
        }

        [TestMethod]
        public void TestComposition()
        {
            var uri =
                new AbsoluteUri(Scheme, Host, Port)
                .AddQueryParameter(FieldName1, FieldValue1)
                .Credentials(Username, Password)
                .AddQueryParameter(FieldName2, FieldValue2)
                .Fragment(Fragment)
                .Path(PathPart1, PathPart2);

            Assert.AreEqual(
                expected,
                uri.ToString());
        }

        [TestMethod]
        public void TestComposition2()
        {
            var uri =
                Host.ToHttpUriFromHost(Port)
                .AddQueryParameter(FieldName1, FieldValue1)
                .Credentials(Username, Password)
                .AddQueryParameter(FieldName2, FieldValue2)
                .Fragment(Fragment)
                .Path(PathPart1, PathPart2);

            Assert.AreEqual(
                expected,
                uri.ToString());
        }

        [TestMethod]
        public void TestComposition3()
        {
            var uri = Host.ToHttpsUriFromHost();

            Assert.AreEqual(
                $"https://{Host}",
                uri.ToString());
        }

        [TestMethod]
        public void TestAbsoluteWithRelative()
        {
            var absolute = new AbsoluteUri(Scheme, Host);

            var relativeRelativeUri = new RelativeUri(
                                    ImmutableList.Create(PathPart1, PathPart2),
                                    ImmutableList.Create(
                                        new QueryParameter(FieldName1, FieldValue1),
                                        new QueryParameter(FieldName2, FieldValue2)
                                        )
                                );

            absolute = absolute.RelativeUri(relativeRelativeUri);

            Assert.AreEqual(
                relativeRelativeUri.Fragment,
                absolute.RelativeUri.Fragment);
        }

        [TestMethod]
        public void TestRelativeWithFragment()
        {
            var relativeRelativeUri = new RelativeUri(
                                    ImmutableList.Create(PathPart1, PathPart2),
                                    ImmutableList.Create(
                                        new QueryParameter(FieldName1, FieldValue1),
                                        new QueryParameter(FieldName2, FieldValue2)
                                        )
                                );

            const string frag = "test";

            relativeRelativeUri = relativeRelativeUri.Fragment(frag);

            Assert.AreEqual(
                frag,
                relativeRelativeUri.Fragment);
        }

        [TestMethod]
        public void TestWithQueryStringStrings()
        {
            var relativeRelativeUri = RelativeUri.Empty.AddQueryString(FieldName1, FieldValue1);

            Assert.AreEqual(
            FieldName1,
            relativeRelativeUri.QueryParameters?.First().FieldName
            );

            Assert.AreEqual(
            FieldValue1,
            relativeRelativeUri.QueryParameters?.First().Value
            );
        }

        [TestMethod]
        public void TestAbsoluteWithQueryStringStrings()
        {
            var absoluteRelativeUri = new AbsoluteUri("https", "test.com");

            absoluteRelativeUri = absoluteRelativeUri.AddQueryParameter(FieldName1, FieldValue1);

            Assert.AreEqual(
            FieldName1,
            absoluteRelativeUri.RelativeUri.QueryParameters.First().FieldName
            );

            Assert.AreEqual(
            FieldValue1,
            absoluteRelativeUri.RelativeUri.QueryParameters.First().Value
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

            var relativeUri = RelativeUri.Empty.QueryParamers(item);

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

            var uri = new Uri(uriString, UriKind.Absolute).ToAbsoluteUri();

            Assert.IsNotNull(uri);
            Assert.AreEqual(uri.Scheme, Scheme);
            Assert.AreEqual(uri.RelativeUri.Fragment, Fragment);
            Assert.AreEqual(uri.RelativeUri.QueryParameters.First().FieldName, FieldName1);
            Assert.AreEqual(uri.RelativeUri.QueryParameters.First().Value, FieldValueEncoded1);
            Assert.AreEqual(uri.RelativeUri.QueryParameters[1].FieldName, FieldName2);
            Assert.AreEqual(uri.RelativeUri.QueryParameters[1].Value, FieldValueEncoded2);
            Assert.AreEqual(Host, uri.Host);
            Assert.AreEqual(Port, uri.Port);
            Assert.AreEqual(PathPart1, uri.RelativeUri.Path[0]);
            Assert.AreEqual(PathPart2, uri.RelativeUri.Path[1]);
            Assert.AreEqual(Fragment, uri.RelativeUri.Fragment);
            Assert.AreEqual(Username, uri.UserInfo?.Username);
            Assert.AreEqual(Password, uri.UserInfo?.Password);
        }

    }


}
