using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.JoshBrunton.DotnetManageSecrets.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
internal class NoopFilter : IFilter
{
    public Result<string> Clean(string input)
    {
        return input;
    }

    public Result<string> Smudge(string input)
    {
        return input;
    }
}
