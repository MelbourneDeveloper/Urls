namespace Uris.TestsFSharp

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Urls

[<TestClass>]
type TestClass () =



    [<TestMethod>]
    member this.TestStringToAbsoluteUri () =

        let uri = "http://username:password@host.com:5000/pathpart1/pathpart2?fieldname1=field%3C%3EValue1&FieldName2=field%3C%3EValue2#frag".ToAbsoluteUrl()

        Assert.AreEqual("frag", uri.RelativeUrl.Fragment)
        Assert.AreEqual("username:password@", uri.UserInfo.ToString())
       

    [<TestMethod>]
    member this.TestComposition () =

        let uri =
         "host.com".ToHttpUriFromHost(5000)
            .AddQueryParameter("fieldname1", "field<>Value1")
            .WithCredentials("username", "password")
            .AddQueryParameter("FieldName2", "field<>Value2")
            .WithFragment("frag")
            .WithPath("pathpart1", "pathpart2")

        Assert.AreEqual("http://username:password@host.com:5000/pathpart1/pathpart2?fieldname1=field%3C%3EValue1&FieldName2=field%3C%3EValue2#frag",uri.ToString());
   
