using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Tests.Tests.Services;

[TestClass]
public class FilterFactoryTests
{
    public static IEnumerable<object[]> GetDataFormats()
    {
        return Enum.GetValues<DataFormats>().Select(value => new object[] { value });
    }

    [TestMethod]
    [DynamicData(nameof(GetDataFormats), DynamicDataSourceType.Method)]
    public void CanGetFilterForDataFormat(DataFormats format)
    {
        try
        {
            _ = FilterFactory.GetFilterForDataFormat(format);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message, ex);
        }
    }

    [TestMethod]
    [DynamicData(nameof(GetDataFormats), DynamicDataSourceType.Method)]
    public void CanGetFileExtensionForDataFormat(DataFormats format)
    {
        try
        {
            _ = FilterFactory.GetFileExtensionForDataFormat(format);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message, ex);
        }
    }
}
