namespace Crudspa.Content.Display.Server.Services;

public class CourseRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ICourseRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Course?>> FetchCourse(Request<Course> request)
    {
        return await wrappers.Try<Course?>(request, async response =>
            await CourseSelectRun.Execute(Connection, request.Value, request.SessionId));
    }

    public async Task<Response<Track?>> FetchTrack(Request<Track> request)
    {
        return await wrappers.Try<Track?>(request, async response =>
            await TrackSelectRun.Execute(Connection, request.Value.Id, request.SessionId));
    }

    public async Task<Response<PortalTracks?>> FetchPortalTracks(Request request)
    {
        return await wrappers.Try<PortalTracks?>(request, async response =>
            await TrackSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<CourseProgress>>> FetchAllProgress(Request request)
    {
        return await wrappers.Try<IList<CourseProgress>>(request, async response =>
            await CourseProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<CourseProgress> FetchProgress(Request<Course> request)
    {
        return await CourseProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddCompleted(Request<CourseCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var courseCompleted = request.Value;

                await CourseCompletedInsert.Execute(connection, transaction, request.SessionId, courseCompleted);

                var allCoursesCompleted = await TrackAllCoursesAreCompleted.Execute(Connection, request.SessionId, null, courseCompleted.CourseId);

                if (allCoursesCompleted)
                    await TrackCompletedInsert.Execute(connection, transaction, request.SessionId, courseCompleted.CourseId);
            });
        });
    }
}