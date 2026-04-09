namespace Crudspa.Education.Publisher.Server.Services;

public class GameActivityServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IActivityService activityService)
    : IGameActivityService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<GameActivity>>> FetchForSection(Request<GameSection> request)
    {
        return await wrappers.Try<IList<GameActivity>>(request, async response =>
        {
            var gameActivities = await GameActivitySelectForSection.Execute(Connection, request.SessionId, request.Value.Id);
            return gameActivities;
        });
    }

    public async Task<Response<GameActivity?>> Fetch(Request<GameActivity> request)
    {
        return await wrappers.Try<GameActivity?>(request, async response =>
        {
            var gameActivity = await GameActivitySelect.Execute(Connection, request.SessionId, request.Value);
            return gameActivity;
        });
    }

    public async Task<Response<GameActivity?>> Add(Request<GameActivity> request)
    {
        return await wrappers.Validate<GameActivity?, GameActivity>(request, async response =>
        {
            var gameActivity = request.Value;

            var activityResponse = await activityService.Add(new(request.SessionId, gameActivity.Activity!));

            if (!activityResponse.Ok)
            {
                response.AddErrors(activityResponse.Errors);
                return null;
            }

            gameActivity.ActivityId = activityResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await GameActivityInsert.Execute(connection, transaction, request.SessionId, gameActivity);

                return new GameActivity
                {
                    Id = id,
                    SectionId = gameActivity.SectionId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<GameActivity> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var gameActivity = request.Value;

            var activityResponse = await activityService.Save(new(request.SessionId, gameActivity.Activity!));

            if (!activityResponse.Ok)
            {
                response.AddErrors(activityResponse.Errors);
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameActivityUpdate.Execute(connection, transaction, request.SessionId, gameActivity);
            });
        });
    }

    public async Task<Response> Remove(Request<GameActivity> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var gameActivity = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameActivityDelete.Execute(connection, transaction, request.SessionId, gameActivity);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<GameActivity>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var gameActivities = request.Value;

            gameActivities.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameActivityUpdateOrdinals.Execute(connection, transaction, request.SessionId, gameActivities);
            });
        });
    }

    public async Task<Response<IList<ActivityTypeFull>>> FetchActivityTypes(Request request)
    {
        return await wrappers.Try<IList<ActivityTypeFull>>(request, async response =>
            await ActivityTypeSelectFull.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchContentAreaNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ContentAreaSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchResearchGroupNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ResearchGroupSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchBooks(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await BookSelectNamesWithGames.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchGameActivityTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GameActivityTypeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchSections(Request<Book> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await GameSectionSelectForBook.Execute(Connection, request.Value.Id));
    }

    public async Task<Response> Share(Request<GameActivityShare> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await GameActivityShareWithSection.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }
}