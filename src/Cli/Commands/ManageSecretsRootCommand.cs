using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Requests.MainService;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;
using System.CommandLine;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Arguments;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Services.GetOutputData;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Commands;

internal class ManageSecretsRootCommand : RootCommand
{
    private readonly IMainService _mainService;
    private readonly IGetOutputDataFactory _getOutputDataFactory;

    private readonly ProjectQueryOption _projectQueryOption;
    private readonly FormatOption _formatOption;
    private readonly EscapeWslOption _escapeWslOption;
    private readonly ObfuscateOption _obfuscateOption;
    private readonly EditorOption _editorOption;
    private readonly ReadonlyOption _readonlyOption;
    private readonly EditorArgumentsArgument _editorArgumentsArgument;

    public ManageSecretsRootCommand(IMainService mainService,
        IGetOutputDataFactory getOutputDataFactory,
        ProjectQueryOption projectQueryOption,
        FormatOption formatOption,
        EscapeWslOption escapeWslOption,
        ObfuscateOption obfuscateOption,
        EditorOption editorOption,
        ReadonlyOption readonlyOption,
        EditorArgumentsArgument editorArgumentsArgument,
        OpenCliCommand openCliCommand,
        DiagnoseCommand diagnoseCommand)
    {
        Description = """
                      Manage dotnet user secrets with your editor and format of choice. 

                      This program reads the user secrets ID of a given .NET project and looks for the associated file in the user secrets folder. It reads the secrets into a sensible format and presents them using the configured editor, then re-formats the edited file into .NET's expected schema before saving. 

                      While editing, a copy of the secrets is stored in the system's temp directory. The file is deleted immediately after closing the editor and loading the new values into memory. Note that the file may persist if the program is not allowed to exit gracefully.

                      TIP: You can configure default arguments to this command by using the file ~/.config/dotnet-manage-secrets.rsp. 
                      """;

        _mainService = mainService;
        _getOutputDataFactory = getOutputDataFactory;

        _projectQueryOption = projectQueryOption;
        _formatOption = formatOption;
        _escapeWslOption = escapeWslOption;
        _obfuscateOption = obfuscateOption;
        _editorOption = editorOption;
        _readonlyOption = readonlyOption;
        _editorArgumentsArgument = editorArgumentsArgument;

        Add(_projectQueryOption);
        Add(_formatOption);
        Add(_escapeWslOption);
        Add(_obfuscateOption);
        Add(_editorOption);
        Add(_readonlyOption);
        Add(_editorArgumentsArgument);

        Add(openCliCommand);
        Add(diagnoseCommand);

        SetAction(ExecuteAction);
    }

    public int Execute(string[] args)
    {
        if (!args.Contains("--no-autorsp"))
        {
            string defaultRspPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                ".config", "dotnet-manage-secrets.rsp");
            if (File.Exists(defaultRspPath))
            {
                args = [.. args, $"@{defaultRspPath}"];
            }
        }

        return Parse(args).Invoke();
    }

    private int ExecuteAction(ParseResult parseResult)
    {
        string projectQuery = parseResult.GetValue(_projectQueryOption) ?? string.Empty;
        DataFormats format = parseResult.GetValue(_formatOption);
        bool escapeWsl = parseResult.GetValue(_escapeWslOption);
        bool obfuscate = parseResult.GetValue(_obfuscateOption);
        string editor = parseResult.GetValue(_editorOption) ?? string.Empty;
        List<string> editorArgs = parseResult.GetValue(_editorArgumentsArgument) ?? [];
        bool readOnly = parseResult.GetValue(_readonlyOption);

        var editContent = _mainService.GetContentForUserEdit(new GetContentForUserEditRequest(projectQuery, format, escapeWsl, obfuscate));

        if (readOnly)
        {
            Console.WriteLine(editContent.Content);
            return 0;
        }

        IGetOutputData dataFetcher = _getOutputDataFactory.GetDataFetcher();
        string contentFromEditor = dataFetcher
            .GetOutputData(editContent.Content, editContent.SuggestedFileExtension, editor, editorArgs)
            .Unwrap();

        var result = _mainService.SetUserSecretsContent(new SetUserSecretsContentRequest(projectQuery, contentFromEditor, format, escapeWsl));

        return result.IsOk ? 0 : (int)result.Error!.Value;
    }
}
