namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IHtmlSanitizer
{
    String? Sanitize(String? html, String? baseUrl = null);
}