using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Localization;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class ProjectQueryOption : Option<string>
{
    private readonly IStringLocalizer<ProjectQueryOptionResources> _localizer;
    private static string ValueFactory(ArgumentResult _) => Directory.GetCurrentDirectory();

    public ProjectQueryOption(IStringLocalizer<ProjectQueryOptionResources> localizer) : base("--project", "-p")
    {
        _localizer = localizer;
        Validators.Add(PathExistsValidator);
        DefaultValueFactory = ValueFactory;
        Description = ProjectQueryOptionResources.Description;
    }

    private void PathExistsValidator(OptionResult opt)
    {
        string? value = opt.GetValue(this);
        if (!Path.Exists(value))
        {
            opt.AddError(_localizer[ProjectQueryOptionResources.PathNotFound, value ?? string.Empty]);
        }
    }
}
