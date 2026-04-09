namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IPageRunService
{
    Task<Response<Page?>> Fetch(Request<Page> request);
    Task<Response> MarkViewed(Request<Page> request);
}