var target = Argument("target", "Pack");

Task("Restore")
    .Does(() => {
        DotNetRestore(".");
    });

Task("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetClean(".");
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetBuild(".");
    });

Task("Pack")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetPack("./src/DotnetManageSecrets", new DotNetPackSettings() 
        {
            Configuration = "Release"
        });
    });

RunTarget(target);