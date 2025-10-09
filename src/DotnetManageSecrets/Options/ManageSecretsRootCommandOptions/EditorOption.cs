using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
internal class EditorOption : Option<string>
{
    private static string ValueFactory(ArgumentResult _) => Environment.GetEnvironmentVariable("EDITOR") ?? "";
    
    public EditorOption() : base("--editor", "-e")
    {
        Validators.Add(ValueIsInvokableBinaryValidator);
        DefaultValueFactory = ValueFactory;
        Description = """
                      The command to invoke the editor. This can be a relative path, fully qualified path, or executable in $PATH. Shell aliases are not supported.
                      """;
    }

    private void ValueIsInvokableBinaryValidator(OptionResult opt)
    {
        var fileName = opt.GetValue(this);

        if (fileName == null)
        {
            opt.AddError("The given editor was null.");
            return;
        }

        if (File.Exists(fileName))
        {
            return;
        }

        var pathFolders = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        if (pathFolders.Split(Path.PathSeparator)
            .Select(path => Path.Combine(path, fileName))
            .Any(File.Exists))
        {
            return;
        }
        
        opt.AddError($"Editor \"{fileName}\" was not a relative or fully qualified path, and was not in $PATH.");
    }
}
