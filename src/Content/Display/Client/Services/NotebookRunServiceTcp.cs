namespace Crudspa.Content.Display.Client.Services;

public class NotebookRunServiceTcp(IProxyWrappers proxyWrappers) : INotebookRunService
{
    public async Task<Response<Notebook?>> FetchNotebookByContact(Request request) =>
        await proxyWrappers.Send<Notebook?>("NotebookRunFetchNotebookByContact", request);

    public async Task<Response> AddNotepage(Request<Notepage> request) =>
        await proxyWrappers.Send("NotebookRunAddNotepage", request);

    public async Task<Response> SaveNotepage(Request<Notepage> request) =>
        await proxyWrappers.Send("NotebookRunSaveNotepage", request);
}