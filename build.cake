var target = Argument("target", "");
var configuration = Argument("configuration", "Release");

Task("InstallSdk").Does(() =>
{
    IEnumerable<FilePath> sdkFiles = GetFiles("./sdk/*.json").Distinct();

    foreach (FilePath sdkFile in sdkFiles)
    {
        if (IsRunningOnWindows())
        {
            StartProcess("pwsh", $"-ExecutionPolicy Bypass -File ./script/dotnet-install.ps1 --jsonfile {sdkFile}");
        }
        else
        {
            StartProcess("bash", $"./script/dotnet-install.sh --jsonfile {sdkFile}");
        }
    }
});

Task("Install").Does(() =>
{
    string nupkgDir = "./src/Cli/bin/nupkg";
    if (DirectoryExists(nupkgDir))
    {
        DeleteDirectory(nupkgDir, new DeleteDirectorySettings { Recursive = true });
    }

    StartProcess("dotnet", "tool uninstall -g DotnetManageSecrets");
    DotNetPack("./src/Cli", new DotNetPackSettings { Configuration = configuration });
    DotNetTool($"tool install -g --add-source {nupkgDir} DotnetManageSecrets --allow-downgrade");
});

RunTarget(target);