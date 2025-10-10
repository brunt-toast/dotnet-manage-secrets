
# Install Locally
```
dotnet cake
dotnet tool install -g --add-source ./src/DotnetManageSecrets/bin/nupkg DotnetManageSecrets
```

# Publish 
```bash
dotnet nuget push \
    <nupkg-file> \
    --api-key <your-key> \ 
    --source https://api.nuget.org/v3/index.json
```

# Configure Completion

```
dotnet tool install -g dotnet-suggest
```
source dotnet-suggest-shim.[[bash](https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Suggest/dotnet-suggest-shim.bash)|[zsh](https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Suggest/dotnet-suggest-shim.zsh)|[ps1](https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Suggest/dotnet-suggest-shim.ps1)]