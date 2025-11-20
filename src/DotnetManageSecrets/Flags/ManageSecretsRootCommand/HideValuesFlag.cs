using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Flags.ManageSecretsRootCommand;

internal class HideValuesFlag : Flag
{
    public HideValuesFlag() : base("--hide-values", "-x")
    {
        Description = "When loading, set all strings, numbers, and booleans to '', 0, and false respectively. " +
                      "Arrays, objects, and nulls remain intact.";
    }
}
