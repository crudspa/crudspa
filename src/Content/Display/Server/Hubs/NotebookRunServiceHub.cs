namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<Notebook?>> NotebookRunFetchNotebookByContact(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await NotebookRunService.FetchNotebookByContact(request));
    }

    public async Task<Response> NotebookRunAddNotepage(Request<Notepage> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await NotebookRunService.AddNotepage(request));
    }

    public async Task<Response> NotebookRunSaveNotepage(Request<Notepage> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await NotebookRunService.SaveNotepage(request));
    }
}