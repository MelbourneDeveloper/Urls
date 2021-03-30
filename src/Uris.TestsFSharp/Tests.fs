namespace Uris.TestsFSharp



open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Uris

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestStringToAbsoluteUri () =

        let uri = "http://username:password@host.com:5000/pathpart1/pathpart2?fieldname1=field%3C%3EValue1&FieldName2=field%3C%3EValue2#frag".ToAbsoluteUri()

        Assert.AreEqual("frag", uri.RelativeUri.Fragment)
        Assert.AreEqual("username:password@", uri.UserInfo.ToString())
       

     
