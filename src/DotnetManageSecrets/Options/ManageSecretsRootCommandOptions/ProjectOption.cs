using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
internal class ProjectOption : Option<string>
{
    private static string ValueFactory(ArgumentResult _) => Directory.GetCurrentDirectory();

    public ProjectOption() : base("--project", "-p")
    {
        Validators.Add(PathExistsValidator);
        DefaultValueFactory = ValueFactory;
        Description = "A .csproj file, or a directory containing at least one .csproj file in its tree. " +
                      "In the case of multiple csproj files, a selection prompt wil appear. ";
    }

    private void PathExistsValidator(OptionResult opt)
    {
        string? value = opt.GetValue(this);
        if (!Path.Exists(value))
        {
            opt.AddError($"The path \"{value}\" does not exist.");
        }
    }
}
