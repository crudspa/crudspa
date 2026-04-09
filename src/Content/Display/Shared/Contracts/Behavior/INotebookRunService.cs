namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface INotebookRunService
{
    Task<Response<Notebook?>> FetchNotebookByContact(Request request);
    Task<Response> AddNotepage(Request<Notepage> request);
    Task<Response> SaveNotepage(Request<Notepage> request);
}