using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Generators.Enums;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Tests.Services.FormatConverters;

[TestClass]
public class JsonFormatConverterTests
{
    [TestMethod]
    public void Clean_ShouldCreateNesting()
    {
        var sut = new JsonFormatConverter();

        string input = """
                       {"l1:l2": "value"}
                       """;

        string formatted = sut.Clean(input).Unwrap();

        var deserialised = JObject.Parse(formatted);
        Assert.AreEqual("value", deserialised["l1"]!["l2"]);
    }

    [TestMethod]
    public void Smudge_ShouldDestroyNesting()
    {
        var sut = new JsonFormatConverter();

        string input = """
                       {"l1": {"l2": "value"}}
                       """;

        string formatted = sut.Smudge(input).Unwrap();
        var deserialised = JObject.Parse(formatted);
        Assert.AreEqual("value", deserialised["l1:l2"]);
    }
}
