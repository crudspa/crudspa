namespace Crudspa.Education.Student.Server.Services;

public class ModuleProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IModuleProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ModuleProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<ModuleProgress>>(request, async response =>
            await ModuleProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<ModuleProgress> Fetch(Request<Module> request)
    {
        return await ModuleProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddCompleted(Request<ModuleCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var moduleCompleted = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ModuleCompletedInsert.Execute(connection, transaction, request.SessionId, moduleCompleted);
            });
        });
    }
}