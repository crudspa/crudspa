namespace Crudspa.Framework.Core.Client.Services;

public class CookieServiceCore(IJsBridge jsBridge) : ICookieService
{
    public async Task<String> Get(String key, String defaultValue = "")
    {
        return await jsBridge.GetCookie(key, defaultValue);
    }

    public async Task Set(String key, String value, DateTimeOffset? expires = null)
    {
        await jsBridge.SetCookie(key, value, expires);
    }
}