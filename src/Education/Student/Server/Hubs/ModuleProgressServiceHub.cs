namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response<IList<ModuleProgress>>> ModuleProgressFetchAll(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ModuleProgressService.FetchAll(request));
    }

    public async Task<Response> ModuleProgressAddCompleted(Request<ModuleCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await ModuleProgressService.AddCompleted(request);

            if (response.Ok)
            {
                var progress = await ModuleProgressService.Fetch(new(session.Id, new() { Id = request.Value.ModuleId }));
                await Notify(session.Id, session.User.Contact.Id, new ModuleProgressUpdated { Progress = progress });

                var moduleAchievementResponse = await StudentAchievementService.CheckModule(new(session.Id, progress));
                if (moduleAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new StudentAchievementAdded { StudentAchievement = moduleAchievementResponse.Value });
            }

            return response;
        });
    }
}