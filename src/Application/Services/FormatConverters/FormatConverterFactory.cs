using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;

internal class FormatConverterFactory : IFormatConverterFactory
{
    public Result<IFormatConverter> GetFilterForDataFormat(DataFormats format)
    {
        return format switch
        {
            DataFormats.Json => new JsonFormatConverter(),
            DataFormats.MinifiedJson => new MinifiedJsonFormatConverter(),
            DataFormats.FlatJson => new NoopFormatConverter(),
            DataFormats.Yaml => new YamlFormatConverter(),
            DataFormats.Xml => new XmlFormatConverter(),
            DataFormats.Toml => new TomlFormatConverter(),
            DataFormats.Ini => new IniFormatConverter(),
            DataFormats.Env => new EnvFormatConverter(),
            _ => ErrorCodes.FactoryHasNoSuitablePath
        };
    }
}

public interface IFormatConverterFactory
{
    Result<IFormatConverter> GetFilterForDataFormat(DataFormats format);
}