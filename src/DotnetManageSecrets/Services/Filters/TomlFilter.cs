using System.Text.Json;
using Dev.JoshBrunton.DotnetManageSecrets.Types;
using Tomlyn;
using Tomlyn.Model;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
internal class TomlFilter : IFilter
{
    private readonly JsonFilter _jsonFilter = new();

    public Result<string> Clean(string input)
    {
        string nestedJson = _jsonFilter.Clean(input).Unwrap();
        return JsonToToml(nestedJson);
    }

    public Result<string> Smudge(string input)
    {
        var nestedJson = TomlToJson(input);
        return _jsonFilter.Smudge(nestedJson);
    }

    private static string JsonToToml(string json)
    {
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var tomlModel = ConvertJsonToToml(data);
        return Toml.FromModel(tomlModel);
    }

    private static string TomlToJson(string toml)
    {
        var model = Toml.ToModel(toml);
        string json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
        return json;
    }

    private static TomlTable ConvertJsonToToml(JsonElement json)
    {
        var table = new TomlTable();
        foreach (var prop in json.EnumerateObject())
        {
            table[prop.Name] = (prop.Value.ValueKind switch
            {
                JsonValueKind.Object => ConvertJsonToToml(prop.Value),
                JsonValueKind.Array => ConvertJsonArray(prop.Value),
                JsonValueKind.String => prop.Value.GetString(),
                JsonValueKind.Number => prop.Value.TryGetInt64(out var i) ? i : prop.Value.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            })!;
        }
        return table;
    }

    private static TomlArray ConvertJsonArray(JsonElement array)
    {
        var tomlArray = new TomlArray();
        foreach (var item in array.EnumerateArray())
        {
            tomlArray.Add(item.ValueKind switch
            {
                JsonValueKind.Object => ConvertJsonToToml(item),
                JsonValueKind.Array => ConvertJsonArray(item),
                JsonValueKind.String => item.GetString(),
                JsonValueKind.Number => item.TryGetInt64(out var i) ? i : item.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            });
        }
        return tomlArray;
    }
}
