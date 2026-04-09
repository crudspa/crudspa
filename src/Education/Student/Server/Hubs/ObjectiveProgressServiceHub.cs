namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response<IList<ObjectiveProgress>>> ObjectiveProgressFetchAll(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ObjectiveProgressService.FetchAll(request));
    }

    public async Task<Response> ObjectiveProgressAddCompleted(Request<ObjectiveCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await ObjectiveProgressService.AddCompleted(request);

            if (response.Ok)
            {
                var progress = await ObjectiveProgressService.Fetch(new(session.Id, new() { Id = request.Value.ObjectiveId }));
                await Notify(session.Id, session.User.Contact.Id, new ObjectiveProgressUpdated { Progress = progress });

                var objectiveAchievementResponse = await StudentAchievementService.CheckObjective(new(session.Id, progress));
                if (objectiveAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new StudentAchievementAdded { StudentAchievement = objectiveAchievementResponse.Value });

                var lessonAchievementResponse = await StudentAchievementService.CheckLesson(new(session.Id, progress));
                if (lessonAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new StudentAchievementAdded { StudentAchievement = lessonAchievementResponse.Value });

                var unitAchievementResponse = await StudentAchievementService.CheckUnit(new(session.Id, progress));
                if (unitAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new StudentAchievementAdded { StudentAchievement = unitAchievementResponse.Value });
            }

            return response;
        });
    }
}