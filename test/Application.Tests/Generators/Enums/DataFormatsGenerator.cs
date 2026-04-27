using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Generators.Enums;

internal static class DataFormatsGenerator
{
    public static IEnumerable<object[]> GetDataFormats()
    {
        return Enum.GetValues<DataFormats>().Select(value => (object[])[value]);
    }
}
