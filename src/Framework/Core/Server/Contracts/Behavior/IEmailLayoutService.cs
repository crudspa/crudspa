namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IEmailLayoutService
{
    Task<String> Fetch(String key);
}