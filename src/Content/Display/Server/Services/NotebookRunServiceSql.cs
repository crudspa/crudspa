namespace Crudspa.Content.Display.Server.Services;

public class NotebookRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : INotebookRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Notebook?>> FetchNotebookByContact(Request request)
    {
        return await wrappers.Try<Notebook?>(request, async response =>
            await ContactNotebookSelectForContact.Execute(Connection, request.SessionId));
    }

    public async Task<Response> AddNotepage(Request<Notepage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var notepage = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await NotepageInsert.Execute(connection, transaction, request.SessionId, notepage);
            });
        });
    }

    public async Task<Response> SaveNotepage(Request<Notepage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var notepage = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await NotepageUpdate.Execute(connection, transaction, request.SessionId, notepage);
            });
        });
    }
}