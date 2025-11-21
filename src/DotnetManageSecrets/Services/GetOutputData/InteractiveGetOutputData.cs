using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.IO;
using System.Diagnostics;
using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using Dev.JoshBrunton.DotnetManageSecrets.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.GetOutputData;

internal class InteractiveGetOutputData : IGetOutputData
{
    private readonly string _editorPath;
    private readonly IList<string> _editorArgs;
    private readonly DataFormats _dataFormat;

    public string Input { get; init; }

    public InteractiveGetOutputData(string input, string editorPath, IList<string> editorArgs, DataFormats dataFormat)
    {
        _editorPath = editorPath;
        _editorArgs = editorArgs;
        _dataFormat = dataFormat;
        Input = input;
    }

    public string GetOutput()
    {
        string fileFormat = FilterFactory.GetFileExtensionForDataFormat(_dataFormat);
        string editingFileName = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.{fileFormat}");
        FileExtensions.Create(editingFileName, Input);

        ProcessStartInfo psi = new()
        {
            FileName = _editorPath,
        };

        psi.ArgumentList.Add(editingFileName);
        foreach (var arg in _editorArgs)
        {
            psi.ArgumentList.Add(arg);
        }

        using Process? proc = Process.Start(psi);
        if (proc is null)
        {
            Console.Error.WriteLine("The editor process failed to start.");
            Environment.ExitCode = ExitCodes.FailedToStartEditor;
            return Input;
        }

        proc.WaitForExit();

        var contentFromEditor = File.ReadAllText(editingFileName);
        File.Delete(editingFileName);

        return contentFromEditor;
    }
}