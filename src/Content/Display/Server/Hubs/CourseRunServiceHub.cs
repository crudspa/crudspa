namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<Course?>> CourseRunFetchCourse(Request<Course> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await CourseRunService.FetchCourse(request));
    }

    public async Task<Response<Track?>> CourseRunFetchTrack(Request<Track> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await CourseRunService.FetchTrack(request));
    }

    public async Task<Response<PortalTracks?>> CourseRunFetchPortalTracks(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await CourseRunService.FetchPortalTracks(request));
    }

    public async Task<Response<IList<CourseProgress>>> CourseRunFetchAllProgress(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await CourseRunService.FetchAllProgress(request));
    }

    public async Task<Response> CourseRunAddCompleted(Request<CourseCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await CourseRunService.AddCompleted(request);

            if (response.Ok)
            {
                var progress = await CourseRunService.FetchProgress(new(session.Id, new() { Id = request.Value.CourseId }));

                await Notify(session.Id, session.User.Contact.Id, new CourseProgressUpdated { Progress = progress });

                var courseAchievementResponse = await ContactAchievementService.CheckCourse(new(session.Id, progress));

                if (courseAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new ContactAchievementAdded { ContactAchievement = courseAchievementResponse.Value });

                var trackAchievementResponse = await ContactAchievementService.CheckTrack(new(session.Id, progress));

                if (trackAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new ContactAchievementAdded { ContactAchievement = trackAchievementResponse.Value });
            }

            return response;
        });
    }
}