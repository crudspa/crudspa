namespace Crudspa.Education.Student.Server.Services;

public class ObjectiveProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IObjectiveProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ObjectiveProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<ObjectiveProgress>>(request, async response =>
            await ObjectiveProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<ObjectiveProgress> Fetch(Request<Objective> request)
    {
        return await ObjectiveProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddCompleted(Request<ObjectiveCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var objectiveCompleted = request.Value;

                await ObjectiveCompletedInsert.Execute(connection, transaction, request.SessionId, objectiveCompleted);

                var allObjectivesCompleted = await LessonAllObjectivesAreCompleted.Execute(Connection, request.SessionId, null, objectiveCompleted.ObjectiveId);

                if (allObjectivesCompleted)
                    await LessonCompletedInsert.Execute(connection, transaction, request.SessionId, objectiveCompleted.ObjectiveId);
            });
        });
    }
}