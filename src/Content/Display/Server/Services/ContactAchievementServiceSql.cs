namespace Crudspa.Content.Display.Server.Services;

public class ContactAchievementServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IContactAchievementService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ContactAchievement>>> FetchAchievements(Request request)
    {
        return await wrappers.Try<IList<ContactAchievement>>(request, async response =>
            await ContactAchievementSelectForSession.Execute(Connection, request.SessionId));
    }

    public async Task<Response<ContactAchievement?>> FetchAchievement(Request<ContactAchievement> request)
    {
        return await wrappers.Try<ContactAchievement?>(request, async response =>
        {
            var contactAchievement = await ContactAchievementSelect.Execute(Connection, request.SessionId, request.Value);

            if (contactAchievement is not null)
                contactAchievement.Unlocks = (await ContactAchievementSelectUnlocks.Execute(Connection, contactAchievement))!;

            return contactAchievement;
        });
    }

    public async Task<Response<ContactAchievement?>> CheckCourse(Request<CourseProgress> request)
    {
        return await wrappers.Try<ContactAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var course = await CourseSelectAchievements.Execute(Connection, progress.CourseId);

            if (course?.GeneratesAchievement?.Id is null)
                return null;

            var contactAchievement = new ContactAchievement
            {
                ContactId = progress.ContactId,
                RelatedEntityId = course.Id,
                Achievement = course.GeneratesAchievement,
            };

            var contactHasAchievement = await ContactAchievementExists.Execute(Connection, contactAchievement);

            if (contactHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                contactAchievement.Id = await ContactAchievementInsert.Execute(connection, transaction, request.SessionId, contactAchievement);
            });

            contactAchievement.IsNew = true;
            return contactAchievement;
        });
    }

    public async Task<Response<ContactAchievement?>> CheckTrack(Request<CourseProgress> request)
    {
        return await wrappers.Try<ContactAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var track = await TrackSelectAchievements.Execute(Connection, progress.CourseId);

            if (track?.GeneratesAchievement?.Id is null)
                return null;

            var allAreComplete = await TrackAllCoursesAreCompleted.Execute(Connection, request.SessionId, track.Id, null);

            if (!allAreComplete)
                return null;

            var contactAchievement = new ContactAchievement
            {
                ContactId = progress.ContactId,
                RelatedEntityId = track.Id,
                Achievement = track.GeneratesAchievement,
            };

            var contactHasAchievement = await ContactAchievementExists.Execute(Connection, contactAchievement);

            if (contactHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                contactAchievement.Id = await ContactAchievementInsert.Execute(connection, transaction, request.SessionId, contactAchievement);
            });

            contactAchievement.IsNew = true;
            return contactAchievement;
        });
    }
}