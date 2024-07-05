using System.Net.Http;
using System.Threading.Tasks;

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
                urlData[url] = response;
            }
            catch (Exception ex)
            {
                urlData[url] = $"Error: {ex.Message}";
            }
        }

        return urlData;
    }
}