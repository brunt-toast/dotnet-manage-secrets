using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class ProjectQueryOption : Option<string>
{
    private static string ValueFactory(ArgumentResult _) => Directory.GetCurrentDirectory();

    public ProjectQueryOption() : base("--project", "-p")
    {
        Validators.Add(PathExistsValidator);
        DefaultValueFactory = ValueFactory;
        Description = "A .*proj file registered for user secrets, " +
                      "or a directory whose tree contains at least one such project. " +
                      "In the case of a directory containing multiple such projects, a selection prompt will appear.";
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
