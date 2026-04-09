namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response<IList<GameProgress>>> GameProgressFetchAll(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await GameProgressService.FetchAll(request));
    }

    public async Task<Response> GameProgressAddCompleted(Request<GameCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await GameProgressService.AddCompleted(request);

            if (response.Ok)
            {
                var progress = await GameProgressService.Fetch(new(session.Id, new() { Id = request.Value.GameId }));
                await Notify(session.Id, session.User.Contact.Id, new GameProgressUpdated { Progress = progress });

                var gameAchievementResponse = await StudentAchievementService.CheckGame(new(session.Id, progress));
                if (gameAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new StudentAchievementAdded { StudentAchievement = gameAchievementResponse.Value });
            }

            return response;
        });
    }
}