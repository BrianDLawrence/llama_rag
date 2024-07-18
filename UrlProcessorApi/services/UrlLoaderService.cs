using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UrlLoaderService : IUrlLoaderService
{
    private readonly HttpClient _httpClient;
    private readonly IContentExtractor _contentExtractor;

    public UrlLoaderService(HttpClient httpClient, IContentExtractor contentExtractor)
    {
        _httpClient = httpClient;
        _contentExtractor = contentExtractor;
    }

    public async Task<Dictionary<string, string>> ProcessUrls(List<string> urls)
    {
        var urlData = new Dictionary<string, string>();

        foreach (var url in urls)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var content = _contentExtractor.ExtractContentFromHtml(response);
                urlData[url] = content;
            }
            catch (Exception ex)
            {
                urlData[url] = $"Error: {ex.Message}";
            }
        }

        return urlData;
    }
}