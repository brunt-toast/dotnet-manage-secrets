using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services;

internal class ValueObfuscator : IValueObfuscator
{
    public string Obfuscate(string json)
    {
        var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? [];
        foreach (var kvp in dict)
        {
            dict[kvp.Key] = kvp.Value switch
            {
                bool _ => false,
                int _ => 0,
                string _ => string.Empty,
                _ => dict[kvp.Key]
            };
        }

        return JsonConvert.SerializeObject(dict);
    }
}

public interface IValueObfuscator
{
    string Obfuscate(string json);
}