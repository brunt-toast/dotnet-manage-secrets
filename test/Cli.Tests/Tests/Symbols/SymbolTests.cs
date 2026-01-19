using System.CommandLine;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Tests.Generators.Symbols;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Tests.Tests.Symbols;

[TestClass] 
public class SymbolTests
{
    [TestMethod]
    [DynamicData(nameof(SymbolGenerator.GetSymbols), typeof(SymbolGenerator))]
    public void Description_ShouldNotBeBlank(Symbol symbol)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(symbol.Description), symbol.GetType().FullName);
    }
}
