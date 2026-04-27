using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services;

internal class UserSecretsIdLocator : IUserSecretsIdLocator
{
    private const string UserSecretsIdAttributeTypeName =
        "Microsoft.Extensions.Configuration.UserSecrets.UserSecretsIdAttribute";
    private const string BuildConfiguration = "Debug";

    public Result<string> TryGetSecretsId(string projectPath)
    {
        Result<string> msBuildResult = TryGetSecretsIdFromMsBuildProperty(projectPath);
        if (msBuildResult.IsOk)
        {
            return msBuildResult;
        }

        Result<string> assemblyPathResult = BuildProjectAndGetTargetAssemblyPath(projectPath);
        if (!assemblyPathResult.IsOk)
        {
            return Result<string>.Err(ErrorCodes.ProjectNotRegisteredForUserSecrets);
        }

        return TryGetSecretsIdFromAssembly(assemblyPathResult.Unwrap());
    }

    private static Result<string> TryGetSecretsIdFromMsBuildProperty(string projectPath)
    {
        string workingDirectory = Path.GetDirectoryName(projectPath) ?? Directory.GetCurrentDirectory();
        string targetsFile = Path.Combine(Path.GetTempPath(), $"dms-{Path.GetRandomFileName()}.targets");
        string idFile = Path.Combine(Path.GetTempPath(), $"dms-{Path.GetRandomFileName()}.txt");

        try
        {
            File.WriteAllText(targetsFile,
                """
                <Project>
                  <Target Name="_DotnetManageSecrets_ExtractUserSecretsId">
                    <WriteLinesToFile File="$(_DotnetManageSecretsIdFile)" Lines="$(UserSecretsId)" Overwrite="true" />
                  </Target>
                </Project>
                """);

            RunDotnet(workingDirectory,
                [
                    "build",
                    projectPath,
                    "--no-restore",
                    "--nologo",
                    "--verbosity", "quiet",
                    "/t:_DotnetManageSecrets_ExtractUserSecretsId",
                    $"/p:Configuration={BuildConfiguration}",
                    $"/p:_DotnetManageSecretsIdFile={idFile}",
                    $"/p:CustomAfterMicrosoftCommonTargets={targetsFile}",
                    $"/p:CustomAfterMicrosoftCommonCrossTargetingTargets={targetsFile}",
                ]);

            if (!File.Exists(idFile))
            {
                return Result<string>.Err(ErrorCodes.ProjectNotRegisteredForUserSecrets);
            }

            string id = File.ReadAllText(idFile).Trim();
            return string.IsNullOrEmpty(id)
                ? Result<string>.Err(ErrorCodes.ProjectNotRegisteredForUserSecrets)
                : Result<string>.Ok(id);
        }
        finally
        {
            TryDeleteFile(targetsFile);
            TryDeleteFile(idFile);
        }
    }

    private static Result<string> BuildProjectAndGetTargetAssemblyPath(string projectPath)
    {
        string workingDirectory = Path.GetDirectoryName(projectPath) ?? Directory.GetCurrentDirectory();
        string targetsFile = Path.Combine(Path.GetTempPath(), $"dms-{Path.GetRandomFileName()}.targets");
        string targetPathFile = Path.Combine(Path.GetTempPath(), $"dms-{Path.GetRandomFileName()}.txt");

        try
        {
            File.WriteAllText(targetsFile,
                """
                <Project>
                  <Target Name="_DotnetManageSecrets_ExtractTargetPath" AfterTargets="Build">
                    <WriteLinesToFile File="$(_DotnetManageSecretsTargetPathFile)" Lines="$(TargetPath)" Overwrite="true" />
                  </Target>
                </Project>
                """);

            bool buildSucceeded = RunDotnet(workingDirectory,
                [
                    "build",
                    projectPath,
                    "--nologo",
                    "--verbosity", "quiet",
                    $"/p:Configuration={BuildConfiguration}",
                    $"/p:_DotnetManageSecretsTargetPathFile={targetPathFile}",
                    $"/p:CustomAfterMicrosoftCommonTargets={targetsFile}",
                    $"/p:CustomAfterMicrosoftCommonCrossTargetingTargets={targetsFile}",
                ]);

            if (!buildSucceeded || !File.Exists(targetPathFile))
            {
                return Result<string>.Err(ErrorCodes.ProjectNotRegisteredForUserSecrets);
            }

            foreach (string candidate in File.ReadAllText(targetPathFile)
                         .Split(['\r', '\n', ';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                string path = Path.IsPathRooted(candidate)
                    ? candidate
                    : Path.GetFullPath(Path.Combine(workingDirectory, candidate));

                if (File.Exists(path))
                {
                    return Result<string>.Ok(path);
                }
            }

            return Result<string>.Err(ErrorCodes.ProjectNotRegisteredForUserSecrets);
        }
        finally
        {
            TryDeleteFile(targetsFile);
            TryDeleteFile(targetPathFile);
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best effort cleanup.
        }
    }

    private static Result<string> TryGetSecretsIdFromAssembly(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        MetadataReader metadataReader = peReader.GetMetadataReader();

        string[] ids = metadataReader.CustomAttributes
            .Select(handle => metadataReader.GetCustomAttribute(handle))
            .Where(attr => IsUserSecretsIdAttribute(metadataReader, attr.Constructor))
            .Select(attr => ReadStringAttributeArgument(metadataReader, attr.Value))
            .OfType<string>()
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (ids.Length != 1)
        {
            return Result<string>.Err(ErrorCodes.ProjectNotRegisteredForUserSecrets);
        }

        return Result<string>.Ok(ids[0]);
    }

    private static bool RunDotnet(string workingDirectory, string[] args)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (string arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }

    private static bool IsUserSecretsIdAttribute(MetadataReader metadataReader, EntityHandle constructor)
    {
        return TryGetAttributeTypeName(metadataReader, constructor, out string attributeTypeName)
               && attributeTypeName == UserSecretsIdAttributeTypeName;
    }

    private static bool TryGetAttributeTypeName(MetadataReader metadataReader, EntityHandle constructor,
        out string attributeTypeName)
    {
        attributeTypeName = string.Empty;

        EntityHandle typeHandle;
        switch (constructor.Kind)
        {
            case HandleKind.MemberReference:
                typeHandle = metadataReader.GetMemberReference((MemberReferenceHandle)constructor).Parent;
                break;
            case HandleKind.MethodDefinition:
                typeHandle = metadataReader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType();
                break;
            default:
                return false;
        }

        switch (typeHandle.Kind)
        {
            case HandleKind.TypeReference:
                TypeReference typeReference = metadataReader.GetTypeReference((TypeReferenceHandle)typeHandle);
                attributeTypeName = $"{metadataReader.GetString(typeReference.Namespace)}.{metadataReader.GetString(typeReference.Name)}";
                return true;

            case HandleKind.TypeDefinition:
                TypeDefinition typeDefinition = metadataReader.GetTypeDefinition((TypeDefinitionHandle)typeHandle);
                attributeTypeName = $"{metadataReader.GetString(typeDefinition.Namespace)}.{metadataReader.GetString(typeDefinition.Name)}";
                return true;

            default:
                return false;
        }
    }

    private static string? ReadStringAttributeArgument(MetadataReader metadataReader, BlobHandle blobHandle)
    {
        BlobReader reader = metadataReader.GetBlobReader(blobHandle);
        const ushort expectedProlog = 1;

        if (reader.ReadUInt16() != expectedProlog)
        {
            return null;
        }

        return reader.ReadSerializedString();
    }
}

internal interface IUserSecretsIdLocator
{
    Result<string> TryGetSecretsId(string projectPath);
}