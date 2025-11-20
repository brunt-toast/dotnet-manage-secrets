using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services;

internal static class ValueObfuscator
{
    public static string Obfuscate(string json)
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
