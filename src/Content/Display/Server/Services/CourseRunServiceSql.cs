namespace Crudspa.Content.Display.Server.Services;

public class CourseRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionLicenseResolver sessionLicenseResolver)
    : ICourseRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Course?>> FetchCourse(Request<Course> request)
    {
        return await wrappers.Try<Course?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await TrackContentIsAccessible.Execute(Connection, licenseIds, courseId: request.Value.Id)
                ? await CourseSelectRun.Execute(Connection, request.Value, request.SessionId)
                : null;
        });
    }

    public async Task<Response<Course?>> FetchCourseForPane(Request<CoursePane> request)
    {
        return await wrappers.Try<Course?>(request, async response =>
        {
            var coursePane = await CoursePaneSelectForPane.Execute(Connection, request.SessionId, request.Value.PaneId);

            var courseId = coursePane?.IdSource == 1
                ? coursePane.CourseId
                : request.Value.RouteCourseId;

            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return courseId.HasValue
                && await TrackContentIsAccessible.Execute(Connection, licenseIds, courseId: courseId)
                ? await CourseSelectRun.Execute(Connection, new() { Id = courseId }, request.SessionId)
                : null;
        });
    }

    public async Task<Response<Track?>> FetchTrack(Request<Track> request)
    {
        return await wrappers.Try<Track?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await TrackContentIsAccessible.Execute(Connection, licenseIds, trackId: request.Value.Id)
                ? await TrackSelectRun.Execute(Connection, request.Value.Id, request.SessionId)
                : null;
        });
    }

    public async Task<Response<PortalTracks?>> FetchPortalTracks(Request request)
    {
        return await wrappers.Try<PortalTracks?>(request, async response =>
        {
            var portalTracks = await TrackSelectAll.Execute(Connection, request.SessionId);
            if (portalTracks is null)
                return null;

            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            foreach (var track in portalTracks.Tracks.ToList())
            {
                if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, trackId: track.Id))
                    portalTracks.Tracks.Remove(track);
            }

            return portalTracks;
        });
    }

    public async Task<Response<IList<CourseProgress>>> FetchAllProgress(Request request)
    {
        return await wrappers.Try<IList<CourseProgress>>(request, async response =>
        {
            var progress = await CourseProgressSelectAll.Execute(Connection, request.SessionId);
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            foreach (var item in progress.ToList())
            {
                if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, courseId: item.CourseId))
                    progress.Remove(item);
            }

            return progress;
        });
    }

    public async Task<CourseProgress> FetchProgress(Request<Course> request)
    {
        var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
        return await TrackContentIsAccessible.Execute(Connection, licenseIds, courseId: request.Value.Id)
            ? await CourseProgressSelect.Execute(Connection, request.SessionId, request.Value.Id)
            : new();
    }

    public async Task<Response> AddCompleted(Request<CourseCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, courseId: request.Value.CourseId))
            {
                response.AddError("Course is not accessible.");
                return;
            }

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