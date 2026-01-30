using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Requests.MainService;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Results.MainService;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    public Result<bool> SetUserSecretsContent(SetUserSecretsContentRequest request)
    {
        IProjectLocator locator = _projectLocatorFactory.GetLocator(request.ProjectQuery).Unwrap();
        IFormatConverter formatConverter = _formatConverterFactory.GetFilterForDataFormat(request.UserContentFormat).Unwrap();

        string projectPath = locator.GetProjectPath(request.ProjectQuery).Unwrap();
        string secretsId = _userSecretsIdLocator.TryGetSecretsId(projectPath).Unwrap();
        string folderLocation = _userSecretsFolderLocator.GetFolderForId(secretsId, request.EscapeWsl);
        string secretsFilePath = Path.Join(folderLocation, "secrets.json");

        string oldContent = _userSecretsReader.ReadUserSecrets(secretsFilePath).Unwrap();
        string newContent = formatConverter.Clean(request.UserContent).Unwrap();

        var oldJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(oldContent) ?? [];
        var newJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(newContent) ?? [];
        if (oldJson.Count == newJson.Count && oldJson.SequenceEqual(newJson))
        {
            return ErrorCodes.LogicalValueHasNotChanged;
        }

        _userSecretsWriter.Write(secretsFilePath, newContent);
        return true;
    }
}

public interface IMainService
{
    GetContentForUserEditResult GetContentForUserEdit(GetContentForUserEditRequest request);
    Result<bool> SetUserSecretsContent(SetUserSecretsContentRequest request);
}