using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services;

internal static class FilterFactory
{
    public static IFilter GetFilterForDataFormat(DataFormats format)
    {
        return format switch
        {
            DataFormats.Json => new JsonFilter(),
            DataFormats.FlatJson => new NoopFilter(),
            DataFormats.Yaml => new YamlFilter(),
            DataFormats.Xml => new XmlFilter(),
            DataFormats.Toml => new TomlFilter(),
            DataFormats.Ini => new IniFilter(),
            DataFormats.Env => new EnvFilter(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static string GetFileExtensionForDataFormat(DataFormats format)
    {
        return format switch
        {
            DataFormats.Json or DataFormats.FlatJson => "json",
            DataFormats.Yaml => "yml",
            DataFormats.Xml => "xml",
            DataFormats.Toml => "toml",
            DataFormats.Ini => "ini",
            DataFormats.Env => "env",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
