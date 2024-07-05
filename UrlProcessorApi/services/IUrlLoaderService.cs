public interface IUrlLoaderService
{
    Task<Dictionary<string, string>> ProcessUrls(List<string> urls);
}