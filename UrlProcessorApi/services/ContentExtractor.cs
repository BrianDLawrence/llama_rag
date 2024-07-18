using HtmlAgilityPack;

public class ContentExtractor : IContentExtractor
{
    public string ExtractContentFromHtml(string html)
    {
        HtmlDocument doc = CreateDocFromHtml(html);
        RemoveScriptAndStyleElements(doc);
        return CleanUpWhiteSpace(GetContentIfItExists(doc));
    }

    private static HtmlDocument CreateDocFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }

    private string GetContentIfItExists(HtmlDocument doc)
    {
        var bodyNode = doc.DocumentNode.SelectSingleNode("//body");
        if (bodyNode == null)
        {
            return string.Empty;
        }

        return bodyNode.InnerText;
    }

    private void RemoveScriptAndStyleElements(HtmlDocument doc)
    {
        doc.DocumentNode.Descendants()
            .Where(n => n.Name == "script" || n.Name == "style")
            .ToList()
            .ForEach(n => n.Remove());
    }

    private string CleanUpWhiteSpace(string extractedText)
    {
        return System.Text.RegularExpressions.Regex.Replace(extractedText, @"\s+", " ").Trim();
    }
}
