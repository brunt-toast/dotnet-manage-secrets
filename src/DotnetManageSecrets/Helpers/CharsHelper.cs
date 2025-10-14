using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets.Helpers;
internal static class CharsHelper
{
    public static string TrimLines(string source, int length = 80)
    {
        StringBuilder ret = new();

        int ptr = 0;
        foreach (string word in source.Split(' '))
        {
            if (ptr > length)
            {
                ret.Append(Environment.NewLine);
                ptr = 0;
            }

            if (word.Contains('\n'))
            {
                ptr = 0;
            }

            ret.Append(word);
            ret.Append(' ');

            ptr += word.Length;
        }

        return ret.ToString();
    }
}
