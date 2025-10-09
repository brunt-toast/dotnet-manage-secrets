using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

namespace Dev.JoshBrunton.DotnetManageSecrets.Tests.Tests.Services.Filters;

[TestClass]
public class FilterTests
{
    public static IEnumerable<object[]> GetFilters()
    {
        var interfaceType = typeof(IFilter);
        var implementations = typeof(IFilter).Assembly
            .GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });

        foreach (var impl in implementations)
        {
            yield return [Activator.CreateInstance(impl) ?? throw new InvalidOperationException()];
        }
    }

    [TestMethod]
    [DynamicData(nameof(GetFilters), DynamicDataSourceType.Method)]
    public void NoOpShouldReturnInput(IFilter instance)
    {
        string input = """
                       {
                         "a": "b"
                       }
                       """;

        string cleaned = instance.Clean(input);
        string smudged = instance.Smudge(cleaned);

        Assert.AreEqual(input, smudged);
    }
}
