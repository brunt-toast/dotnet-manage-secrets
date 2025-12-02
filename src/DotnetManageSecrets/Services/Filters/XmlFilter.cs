using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Dev.JoshBrunton.DotnetManageSecrets.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

internal class XmlFilter : IFilter
{
    private readonly JsonFilter _jsonFilter = new();

    public Result<string> Clean(string input)
    {
        string nestedJson = _jsonFilter.Clean(input).Unwrap();
        return JsonToXml(nestedJson);
    }

    public Result<string> Smudge(string input)
    {
        var nestedJson = XmlToJson(input);
        return _jsonFilter.Smudge(nestedJson);
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
