using System.CommandLine;

namespace Dev.JoshBrunton.DotnetManageSecrets.IO;

internal class ConsoleDiversion : IDisposable
{
    private readonly TextWriter _beforeOut = Console.Out;
    private readonly TextWriter _beforeErr = Console.Error;

    private ConsoleDiversion()
    {
    }

    public static IDisposable ForParseResult(ParseResult result)
    {
        return new ConsoleDiversion()
            .Init(result.InvocationConfiguration.Output, result.InvocationConfiguration.Error);
    }

    private IDisposable Init(TextWriter stdOut, TextWriter stdErr)
    {
        Console.SetOut(stdOut);
        Console.SetError(stdErr);
        return this;
    }

    public void Dispose()
    {
        Console.SetOut(_beforeOut);
        Console.SetError(_beforeErr);
    }
}

