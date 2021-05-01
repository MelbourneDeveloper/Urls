# Urls

Treat Urls as first-class citizens

[Nuget](https://www.nuget.org/packages/Urls) 

| .NET Framework 4.5 | .NET Standard 2.0 | .NET Core 5.0 |
|--------------------|:-----------------:|---------------|

Urls is a .NET library of [records](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records) that represent Urls. All properties are immutable, and there are a collection of Fluent API style extension methods to make Url construction easy. I designed this library with F# in mind. Use the [non-destructive mutation](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records#non-destructive-mutation) (`with`) syntax to create new Urls easily and make HTTP calls with [RestClient.Net](https://github.com/MelbourneDeveloper/RestClient.Net/tree/5/develop). 

#### C#

```cs
private readonly string expected = $"{Scheme}://{Username}:{Password}@{Host}:{Port}/{PathPart1}/{PathPart2}?" +
  $"{FieldName1}={FieldValueEncoded1}&{FieldName2}={FieldValueEncoded2}#{Fragment}";

[TestMethod]
public void TestComposition()
{
    var absoluteUrl =
        Host.ToHttpUriFromHost(Port)
        .AddQueryParameter(FieldName1, FieldValue1)
        .WithCredentials(Username, Password)
        .AddQueryParameter(FieldName2, FieldValue2)
        .WithFragment(Fragment)
        .WithPath(PathPart1, PathPart2);

    Assert.AreEqual(
        expected,
        absoluteUrl.ToString());

    //C# 9 records non-destructive mutation (with syntax)
    var absoluteUrl2 = absoluteUrl with { Port = 1000 };

    Assert.AreEqual(1000, absoluteUrl2.Port);
}
```

#### F#

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

#### Quality First

![Code Coverage](https://github.com/MelbourneDeveloper/Urls/blob/main/Images/CodeCoverage.png) 
![Mutation Score](https://github.com/MelbourneDeveloper/Urls/blob/main/Images/MutationScore.png)



