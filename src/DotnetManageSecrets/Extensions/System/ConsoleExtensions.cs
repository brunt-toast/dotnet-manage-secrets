using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Extensions.System;

internal static class ConsoleExtensions
{
    extension (Console)
    {
        public static string GetPipedInput()
        {
            return Console.IsInputRedirected
                ? Console.In.ReadToEnd()
                : string.Empty;
        }
    }
}
