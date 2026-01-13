using System.Xml;
using System.Xml.Linq;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;

internal class XmlFormatConverter : IFormatConverter
{
    public string SuggestedFileExtension => "xml";

    private readonly JsonFormatConverter _jsonFormatConverter = new();

    public Result<string> Clean(string input)
    {
        string nestedJson = _jsonFormatConverter.Clean(input).Unwrap();
        return JsonToXml(nestedJson);
    }

    public Result<string> Smudge(string input)
    {
        var nestedJson = XmlToJson(input);
        return _jsonFormatConverter.Smudge(nestedJson);
    }

    private string JsonToXml(string input)
    {
        XNode node = JsonConvert.DeserializeXNode(input) ?? throw new ArgumentNullException(input);
        return node.ToString();
    }

    private string XmlToJson(string input)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(input);
        return JsonConvert.SerializeXmlNode(doc);
    }
}
