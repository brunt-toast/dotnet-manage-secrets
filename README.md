
# Install Locally
```
dotnet cake
dotnet tool install -g --add-source ./src/DotnetManageSecrets/nupkg DotnetManageSecrets
```

# Publish 
```bash
dotnet nuget push \
    <nupkg-file> \
    --api-key <your-key> \ 
    --source https://api.nuget.org/v3/index.json
```
