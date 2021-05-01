# Urls

Treat Urls as first class citizens

[Nuget](https://www.nuget.org/packages/Urls)

Urls is a .NET library of [records](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records) that represent Urls. All properties are immutable and there are a collection of Fluent API style extension methods to make Url construction easy. Use the `with` syntax to create new Urls easily, and make Http calls with [RestClient.Net](https://github.com/MelbourneDeveloper/RestClient.Net/tree/5/develop). 

```cs
private readonly string expected = $"{Scheme}://{Username}:{Password}@{Host}:{Port}/{PathPart1}/{PathPart2}?" +
  $"{FieldName1}={FieldValueEncoded1}&{FieldName2}={FieldValueEncoded2}#{Fragment}";

[TestMethod]
public void TestComposition2()
{
    var absoluteUri =
        Host.ToHttpUriFromHost(Port)
        .AddQueryParameter(FieldName1, FieldValue1)
        .WithCredentials(Username, Password)
        .AddQueryParameter(FieldName2, FieldValue2)
        .WithFragment(Fragment)
        .WithPath(PathPart1, PathPart2);

    Assert.AreEqual(
        expected,
        absoluteUri.ToString());
}
```

```fs
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
```

