namespace Crudspa.Content.Display.Server.Contracts.Behavior;

public interface ISeoService
{
    Task<SeoPage> FetchPage(String? path, String requestBaseUrl);
    Task<String> FetchRobotsText(String requestBaseUrl);
    Task<String> FetchSitemapXml(String requestBaseUrl);
}