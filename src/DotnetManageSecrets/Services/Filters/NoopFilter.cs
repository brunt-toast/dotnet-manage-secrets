using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
internal class NoopFilter : IFilter
{
    public string Clean(string input)
    {
        return input;
    }

    public string Smudge(string input)
    {
        return input;
    }
}
