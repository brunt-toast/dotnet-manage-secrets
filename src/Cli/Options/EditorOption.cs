using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Localization;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class EditorOption : Option<string>
{
    private readonly IStringLocalizer<EditorOptionResources> _localizer;
    private static string ValueFactory(ArgumentResult _) => Environment.GetEnvironmentVariable("EDITOR") ?? "";

    public EditorOption(IStringLocalizer<EditorOptionResources> localizer) : base("--editor", "-e")
    {
        _localizer = localizer;
        Validators.Add(ValueIsInvokableBinaryValidator);
        DefaultValueFactory = ValueFactory;
        Description = EditorOptionResources.Description;
    }

    private void ValueIsInvokableBinaryValidator(OptionResult opt)
    {
        var fileName = opt.GetValue(this);

        if (fileName == string.Empty)
        {
            return;
        }

        if (fileName == null)
        {
            opt.AddError(EditorOptionResources.EditorWasNull);
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

        opt.AddError(_localizer[EditorOptionResources.PathNotFound, fileName]);
    }
}
