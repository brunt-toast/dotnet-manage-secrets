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
        Description = """
                      A .*proj file containing a single string matching "<UserSecretsId>{guid}</UserSecretsId>, or a directory whose tree contains at least one such file. In the case of a directory containing multiple such files, a selection prompt will appear. 
                      """;
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
