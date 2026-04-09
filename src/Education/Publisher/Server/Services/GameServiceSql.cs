using AchievementSelectNames = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelectNames;

namespace Crudspa.Education.Publisher.Server.Services;

public class GameServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IGameService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Game>>> SearchForBook(Request<GameSearch> request)
    {
        return await wrappers.Try<IList<Game>>(request, async response =>
        {
            return await GameSelectWhereForBook.Execute(Connection, request.SessionId, request.Value);
        });
    }

    public async Task<Response<Game?>> Fetch(Request<Game> request)
    {
        return await wrappers.Try<Game?>(request, async response =>
        {
            var game = await GameSelect.Execute(Connection, request.SessionId, request.Value);
            return game;
        });
    }

    public async Task<Response<Game?>> Add(Request<Game> request)
    {
        return await wrappers.Validate<Game?, Game>(request, async response =>
        {
            var game = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await GameInsert.Execute(connection, transaction, request.SessionId, game);

                return new Game
                {
                    Id = id,
                    BookId = game.BookId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Game> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var game = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameUpdate.Execute(connection, transaction, request.SessionId, game);
            });
        });
    }

    public async Task<Response> Remove(Request<Game> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var game = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameDelete.Execute(connection, transaction, request.SessionId, game);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request)
    {
        return await wrappers.Try<IList<IconFull>>(request, async response =>
            await IconSelectFull.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GradeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchAssessmentLevelNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await AssessmentLevelSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AchievementSelectNames.Execute(Connection));
    }
}