# dotnet-manage-secrets 

Manage your .NET user secrets using your format and editor of choice! 

[![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=fff)](#)
[![JSON](https://img.shields.io/badge/JSON-000?logo=json&logoColor=fff)](#)
[![YAML](https://img.shields.io/badge/YAML-CB171E?logo=yaml&logoColor=fff)](#)
[![XML](https://img.shields.io/badge/XML-767C52?logo=xml&logoColor=fff)](#)
[![TOML](https://img.shields.io/badge/TOML-9C4121?logo=toml&logoColor=fff)](#)
[![Windows](https://custom-icon-badges.demolab.com/badge/Windows-0078D6?logo=windows11&logoColor=white)](#)
[![macOS](https://img.shields.io/badge/macOS-000000?logo=apple&logoColor=F0F0F0)](#)
[![Linux](https://img.shields.io/badge/Linux-FCC624?logo=linux&logoColor=black)](#)
[![FreeBSD](https://img.shields.io/badge/FreeBSD-AB2B28?logo=freebsd&logoColor=fff)](#)
[![NuGet](https://img.shields.io/badge/NuGet-004880?logo=nuget&logoColor=fff)](#)

## Installation

Install as a .NET tool: 
```sh
dotnet tool install -g DotnetManageSecrets
```

## About

[User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) are a great tool to manage sensitive configuration options while minimising the risk of accidentally committing them to version control, but the `dotnet user-secrets` command can be unintuitive and slow to use. 

This tool is inspired by the Visual Studio secrets editor, which allows you to edit the underlying secrets file (`~/.microsoft/usersecrets/<GUID>/secrets.json`) directly, but only using its native "flattened json" format, and only for project types supported by the Connected Services feature. 

![Visual Studio's secrets editor](assets/vsSecretsEditor.png)

This tool makes the experience far more intuitive by presenting the data in more sensible formats, such as nested JSON or YAML. 

<p float="left">
  <img src="assets/vsNestedSecrets.png" width="49%" style="vertical-align: top;" />
  <img src="assets/vsYamlSecrets.png" width="49%" style="vertical-align: top;" />
</p>

Additionally, it extends compatibility to all major editors - just set `$EDITOR` or pass `--editor|-e`. Here it is working with Neovim: 

![Using Visual Studio to edit user secrets in YAML format](assets/dotnetManageSecretsNvim.png)
<sup>Legend has it that this author was stuck in vim forever more...</sup>

## License 

This tool is published under the [MIT License](./LICENSE.md). 

## Credits

Badges from [inttter/md-badges](https://github.com/inttter/md-badges)