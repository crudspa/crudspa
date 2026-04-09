namespace Crudspa.Framework.Core.Client.Services;

public class UriProviderCore(NavigationManager navigationManager) : IUriProvider
{
    public String Root()
    {
        return navigationManager.BaseUri.RemoveTrailingSlash();
    }
}