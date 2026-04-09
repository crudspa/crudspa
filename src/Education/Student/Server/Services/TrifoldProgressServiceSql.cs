namespace Crudspa.Education.Student.Server.Services;

public class TrifoldProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ITrifoldProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<TrifoldProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<TrifoldProgress>>(request, async response =>
            await TrifoldProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<TrifoldProgress> Fetch(Request<Trifold> request)
    {
        return await TrifoldProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddCompleted(Request<TrifoldCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var trifoldCompleted = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrifoldCompletedInsert.Execute(connection, transaction, request.SessionId, trifoldCompleted);

                var allAreComplete = await TrifoldAllAreComplete.Execute(Connection, request.SessionId, trifoldCompleted.TrifoldId);

                if (allAreComplete)
                    await MapCompletedInsertByTrifold.Execute(connection, transaction, request.SessionId, trifoldCompleted.TrifoldId);
            });
        });
    }
}