namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IThemeRunService
{
    Task<String?> Fetch(Guid portalId, Int32 revision, String build);
    Task<Int32?> FetchRevision(Guid portalId);
    Task<String?> Preview(Guid portalId, String scope);
    Task Warm(Guid portalId);
}