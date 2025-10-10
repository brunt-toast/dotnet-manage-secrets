using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
internal class EditorOption : Option<string>
{
    private string ValueFactory(ArgumentResult _) => Environment.GetEnvironmentVariable("EDITOR") ?? "";
    
    public EditorOption() : base("--editor", "-e")
    {
        DefaultValueFactory = ValueFactory;
        Description = "The command to invoke the editor. This can be a relative path, fully qualified path, or executable in $PATH. " +
                      "Accepts the executable name only - no additional args.";
    }
}
