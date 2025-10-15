# dotnet-manage-secrets 

Manage your .NET user secrets as JSON with the editor of your choice! 

[User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) are a great tool to manage sensitive configuration options while minimising the risk of accidentally committing them to version control, but the `dotnet user-secrets` command can be unintuitive and slow to use. You can edit the underlying file directly, but it's often hard to find, and the flattened JSON schema can make it difficult to navigate. 

The dotnet manage-secrets tool makes the user secrets experience faster and more intuitive by rendering your secrets in a nested JSON format and presenting them in the editor of your choice. 

# Quick Install/Upgrade

We're not available as a tool on NuGet or other package managers yet. Install the tool manually by compiling from source: 

```bash
dotnet tool uninstall -g DotnetManageSecrets; dotnet cake && dotnet tool install -g --add-source ./src/DotnetManageSecrets/bin/nupkg DotnetManageSecrets
```

## License 

This tool is published under the [MIT License](./LICENSE.md). 