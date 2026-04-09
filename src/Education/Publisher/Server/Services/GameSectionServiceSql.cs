namespace Crudspa.Education.Publisher.Server.Services;

public class GameSectionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IGameSectionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<GameSection>>> FetchForGame(Request<Game> request)
    {
        return await wrappers.Try<IList<GameSection>>(request, async response =>
        {
            var gameSections = await GameSectionSelectForGame.Execute(Connection, request.SessionId, request.Value.Id);
            return gameSections;
        });
    }

    public async Task<Response<GameSection?>> Fetch(Request<GameSection> request)
    {
        return await wrappers.Try<GameSection?>(request, async response =>
        {
            var gameSection = await GameSectionSelect.Execute(Connection, request.SessionId, request.Value);
            return gameSection;
        });
    }

    public async Task<Response<GameSection?>> Add(Request<GameSection> request)
    {
        return await wrappers.Validate<GameSection?, GameSection>(request, async response =>
        {
            var gameSection = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await GameSectionInsert.Execute(connection, transaction, request.SessionId, gameSection);

                return new GameSection
                {
                    Id = id,
                    GameId = gameSection.GameId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<GameSection> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var gameSection = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameSectionUpdate.Execute(connection, transaction, request.SessionId, gameSection);
            });
        });
    }

    public async Task<Response> Remove(Request<GameSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var gameSection = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameSectionDelete.Execute(connection, transaction, request.SessionId, gameSection);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<GameSection>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var gameSections = request.Value;

            gameSections.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameSectionUpdateOrdinals.Execute(connection, transaction, request.SessionId, gameSections);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchGameSectionTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GameSectionTypeSelectOrderables.Execute(Connection));
    }
}