using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Generators.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Tests.Services.FormatConverters;

[TestClass]
public class FormatConverterFactoryTests
{
    [TestMethod]
    [DynamicData(nameof(DataFormatsGenerator.GetDataFormats), typeof(DataFormatsGenerator))]
    public void GetFilterForDataFormat_ForValidDataFormats_ShouldReturnValue(DataFormats format)
    {
        var sut = new FormatConverterFactory();
        var result = sut.GetFilterForDataFormat(format);
        Assert.IsTrue(result.IsOk);
    }

    [TestMethod]
    public void GetFilterForDataFormat_ForInvalidDataFormat_ShouldReturnValue()
    {
        var sut = new FormatConverterFactory();
        var result = sut.GetFilterForDataFormat((DataFormats)int.MaxValue);
        Assert.IsFalse(result.IsOk);
    }
}
