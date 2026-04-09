namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface ICookieService
{
    public Task<String> Get(String key, String defaultValue = "");
    public Task Set(String key, String value, DateTimeOffset? expires = null);
}