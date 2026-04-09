namespace Crudspa.Education.Student.Server.Services;

public class GameProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IGameProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<GameProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<GameProgress>>(request, async response =>
            await GameProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<GameProgress> Fetch(Request<Game> request)
    {
        return await GameProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddCompleted(Request<GameCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var gameCompleted = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameCompletedInsert.Execute(connection, transaction, request.SessionId, gameCompleted);
            });
        });
    }
}