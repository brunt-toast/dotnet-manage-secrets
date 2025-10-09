using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets;

internal class ArgsHelper
{
    private readonly List<string> _args;

    public ArgsHelper(string[] args)
    {
        _args = args.ToList();
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        int keyIndex = _args.IndexOf("key");

        if (keyIndex == -1 || _args.Count < keyIndex + 2)
        {
            value = null;
            return false;
        }

        value = _args[keyIndex + 1];
        return true;
    }

    public IEnumerable<string> GetValues(string key)
    {
        for (int i = 0; i < _args.Count; i++)
        {
            if (i == _args.Count - 2)
            {
                continue;
            }

            if (_args[i] == key)
            {
                yield return _args[i + 1];
            }
        }
    }
}
