using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class UrlLoaderService : IUrlLoaderService
{
    private readonly HttpClient _httpClient;

    public UrlLoaderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<string, string>> ProcessUrls(List<string> urls)
    {
        var urlData = new Dictionary<string, string>();

        foreach (var url in urls)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var content = ExtractContentFromHtml(response);
                urlData[url] = content;
            }
            catch (Exception ex)
            {
                urlData[url] = $"Error: {ex.Message}";
            }
        }

        return urlData;
    }

    private string ExtractContentFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Remove script and style elements
        doc.DocumentNode.Descendants()
            .Where(n => n.Name == "script" || n.Name == "style")
            .ToList()
            .ForEach(n => n.Remove());

        // Extract the inner text from the remaining HTML
        return doc.DocumentNode.InnerText;
    }
}
