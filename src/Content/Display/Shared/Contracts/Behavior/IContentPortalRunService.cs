namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IContentPortalRunService
{
    Task<Response<ContentPortal?>> Fetch(Request<ContentPortal> request);
}