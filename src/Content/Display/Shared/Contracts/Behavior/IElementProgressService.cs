namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IElementProgressService
{
    Task<Response<IList<ElementProgress>>> FetchAll(Request request);
    Task<ElementProgress> Fetch(Request<Element> request);
    Task<Response> AddCompleted(Request<ElementCompleted> request);
    Task<Response> AddLink(Request<ElementLink> request);
}