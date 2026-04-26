namespace Crudspa.Framework.Core.Client.Services;

public class CookieServiceCore(IJsBridge jsBridge, NavigationManager navigationManager) : ICookieService
{
    public async Task<String> Get(String key, String defaultValue = "")
    {
        var resolvedKey = Constants.CookieKeys.Resolve(key, new(navigationManager.BaseUri));
        return await jsBridge.GetCookie(resolvedKey, defaultValue);
    }

    public async Task Set(String key, String value, DateTimeOffset? expires = null)
    {
        var resolvedKey = Constants.CookieKeys.Resolve(key, new(navigationManager.BaseUri));
        await jsBridge.SetCookie(resolvedKey, value, expires);
    }
}