namespace Crudspa.Education.Student.Server.Services;

public class BookProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IBookProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<BookProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<BookProgress>>(request, async response =>
            await BookProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<BookProgress> Fetch(Request<Book> request)
    {
        return await BookProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddPrefaceCompleted(Request<PrefaceCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var prefaceCompleted = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PrefaceCompletedInsert.Execute(connection, transaction, request.SessionId, prefaceCompleted);
            });
        });
    }
}