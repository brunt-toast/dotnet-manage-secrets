using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using System.Diagnostics;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Services.GetOutputData;

internal class EditorGetOutputData : IGetOutputData
{
    public Result<string> GetOutputData(string inData, string recommendedFileType, string editor, IEnumerable<string> editorArgs)
    {
        string editingFileName = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.{recommendedFileType}");
        File.WriteAllText(editingFileName, inData);

        ProcessStartInfo psi = new()
        {
            FileName = editor,
        };

        psi.ArgumentList.Add(editingFileName);
        foreach (var arg in editorArgs)
        {
            psi.ArgumentList.Add(arg);
        }

        using Process? proc = Process.Start(psi);
        if (proc is null)
        {
            Console.Error.WriteLine("The editor process failed to start.");
            return ErrorCodes.UnknownError;
        }

        proc.WaitForExit();

        var contentFromEditor = File.ReadAllText(editingFileName);
        File.Delete(editingFileName);

        return contentFromEditor;
    }
}
