using Dev.JoshBrunton.DotnetManageSecrets.Application.Requests.MainService;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Results.MainService;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services;

internal class MainService : IMainService
{
    private readonly IProjectLocatorFactory _projectLocatorFactory;
    private readonly IUserSecretsIdLocator _userSecretsIdLocator;
    private readonly IUserSecretsFolderLocator _userSecretsFolderLocator;
    private readonly IUserSecretsReader _userSecretsReader;
    private readonly IFormatConverterFactory _formatConverterFactory;
    private readonly IValueObfuscator _valueObfuscator;
    private readonly IUserSecretsWriter _userSecretsWriter;

    public MainService(IProjectLocatorFactory projectLocatorFactory,
        IUserSecretsIdLocator userSecretsIdLocator,
        IUserSecretsFolderLocator userSecretsFolderLocator,
        IUserSecretsReader userSecretsReader,
        IFormatConverterFactory formatConverterFactory,
        IValueObfuscator valueObfuscator,
        IUserSecretsWriter userSecretsWriter)
    {
        _projectLocatorFactory = projectLocatorFactory;
        _userSecretsIdLocator = userSecretsIdLocator;
        _userSecretsFolderLocator = userSecretsFolderLocator;
        _userSecretsReader = userSecretsReader;
        _formatConverterFactory = formatConverterFactory;
        _valueObfuscator = valueObfuscator;
        _userSecretsWriter = userSecretsWriter;
    }

    public GetContentForUserEditResult GetContentForUserEdit(GetContentForUserEditRequest request)
    {
        IProjectLocator locator = _projectLocatorFactory.GetLocator(request.ProjectQuery).Unwrap();
        IFormatConverter formatConverter = _formatConverterFactory.GetFilterForDataFormat(request.DesiredFormat).Unwrap();

        string projectPath = locator.GetProjectPath(request.ProjectQuery).Unwrap();
        string secretsId = _userSecretsIdLocator.TryGetSecretsId(projectPath).Unwrap();
        string folderLocation = _userSecretsFolderLocator.GetFolderForId(secretsId, request.EscapeWsl);
        string secretsFilePath = Path.Join(folderLocation, "secrets.json");
        string secretsContent = _userSecretsReader.ReadUserSecrets(secretsFilePath).Unwrap();

        if (request.Obfuscate)
        {
            secretsContent = _valueObfuscator.Obfuscate(secretsContent);
        }

        string preppedContent = formatConverter.Smudge(secretsContent).Unwrap();

        return new GetContentForUserEditResult(preppedContent, formatConverter.SuggestedFileExtension);
    }

    public void SetUserSecretsContent(SetUserSecretsContentRequest request)
    {
        IProjectLocator locator = _projectLocatorFactory.GetLocator(request.ProjectQuery).Unwrap();
        IFormatConverter formatConverter = _formatConverterFactory.GetFilterForDataFormat(request.UserContentFormat).Unwrap();

        string projectPath = locator.GetProjectPath(request.ProjectQuery).Unwrap();
        string secretsId = _userSecretsIdLocator.TryGetSecretsId(projectPath).Unwrap();
        string folderLocation = _userSecretsFolderLocator.GetFolderForId(secretsId, request.EscapeWsl);
        string secretsFilePath = Path.Join(folderLocation, "secrets.json");

        string preppedContent = formatConverter.Clean(request.UserContent).Unwrap();
        _userSecretsWriter.Write(secretsFilePath, preppedContent);
    }
}

public interface IMainService
{
    GetContentForUserEditResult GetContentForUserEdit(GetContentForUserEditRequest request);
    void SetUserSecretsContent(SetUserSecretsContentRequest request);
}