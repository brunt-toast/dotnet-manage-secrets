using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.IO;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace Dev.JoshBrunton.DotnetManageSecrets.Help;

internal class HelpActionWithExitCodes : SynchronousCommandLineAction
{
    private readonly HelpAction _defaultHelp;

    public HelpActionWithExitCodes(HelpAction action) => _defaultHelp = action;

    public override int Invoke(ParseResult parseResult)
    {
        int result = _defaultHelp.Invoke(parseResult);

        using var _ = ConsoleDiversion.ForParseResult(parseResult);
        Console.WriteLine("Exit codes:");

        var intValues = Enum.GetValues<ExitCodes>().Select(x => (int)x).Order().ToList();
        int padding = Math.Max(intValues.First().ToString().Length, intValues.Last().ToString().Length);

        Dictionary<string, string> x = Enum.GetValues<ExitCodes>()
            .Select(value => new KeyValuePair<string, string>(
                $"{((int)value).ToString().PadRight(padding)} {GetName(value)}",
                GetDescription(value)
            )).ToDictionary();

        int lColWidth = x.Keys.MaxBy(x => x.Length).Length;
        int rColWidth = Console.BufferWidth - 6 - lColWidth;

        foreach (var kvp in x)
        {
            PrintColumns(kvp.Key.PadRight(lColWidth), kvp.Value, lColWidth, rColWidth);
        }

        return result;
    }

    private static string GetName(Enum value)
    {
        var member = value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }

    private static string GetDescription(Enum value)
    {
        var member = value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        return member?.GetCustomAttribute<DisplayAttribute>()?.GetDescription() ?? string.Empty;
    }

    private static void PrintColumns(
        string leftText,
        string rightText,
        int leftWidth,
        int rightWidth)
    {
        var leftLines = WrapToWidth(leftText, leftWidth);
        var rightLines = WrapToWidth(rightText, rightWidth);

        int lineCount = Math.Max(leftLines.Count, rightLines.Count);

        for (int i = 0; i < lineCount; i++)
        {
            string left = i < leftLines.Count ? leftLines[i] : "";
            string right = i < rightLines.Count ? rightLines[i] : "";

            Console.WriteLine(
                left.PadRight(leftWidth) + " " + 
                right.PadRight(rightWidth));
        }
    }

    private static List<string> WrapToWidth(string text, int maxWidth)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(text))
        {
            return result;
        }

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var line = new StringBuilder();

        foreach (var word in words)
        {
            if (word.Length > maxWidth)
            {
                if (line.Length > 0)
                {
                    result.Add(line.ToString());
                    line.Clear();
                }

                int start = 0;
                while (start < word.Length)
                {
                    int len = Math.Min(maxWidth, word.Length - start);
                    result.Add(word.Substring(start, len));
                    start += len;
                }
            }
            else
            {
                if (line.Length == 0)
                {
                    line.Append(word);
                }
                else if (line.Length + 1 + word.Length <= maxWidth)
                {
                    line.Append(' ').Append(word);
                }
                else
                {
                    result.Add(line.ToString());
                    line.Clear();
                    line.Append(word);
                }
            }
        }

        if (line.Length > 0)
        {
            result.Add(line.ToString());
        }

        return result;
    }
}
