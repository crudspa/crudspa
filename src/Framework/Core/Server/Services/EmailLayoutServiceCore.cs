namespace Crudspa.Framework.Core.Server.Services;

public class EmailLayoutServiceCore(IEmbeddedResourceService embeddedResourceService) : IEmailLayoutService
{
    public async Task<String> Fetch(String key)
    {
        var assembly = typeof(EmailLayoutServiceCore).Assembly;

        switch (key)
        {
            case Constants.EmailLayoutKeys.AccessCode:
                return await embeddedResourceService.ReadText(assembly, "Crudspa.Framework.Core.Server.Embedded.Templates.EmailSimple.html");

            case Constants.EmailLayoutKeys.Message:
                return await embeddedResourceService.ReadText(assembly, "Crudspa.Framework.Core.Server.Embedded.Templates.EmailSimple.html");

            default:
                throw new($"Email layout '{key}' not found.");
        }
    }
}