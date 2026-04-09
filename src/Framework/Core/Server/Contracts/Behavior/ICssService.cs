namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ICssService
{
    Task<String?> Fetch(String build);
    Task<String?> Bundle();
    Task<String?> Theme(String selector, String? overrides = null);
    String Tag(String build);
}