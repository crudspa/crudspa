namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response<IList<TrifoldProgress>>> TrifoldProgressFetchAll(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await TrifoldProgressService.FetchAll(request));
    }

    public async Task<Response> TrifoldProgressAddCompleted(Request<TrifoldCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await TrifoldProgressService.AddCompleted(request);

            if (response.Ok)
            {
                var progress = await TrifoldProgressService.Fetch(new(session.Id, new() { Id = request.Value.TrifoldId }));
                await Notify(session.Id, session.User.Contact.Id, new TrifoldProgressUpdated { Progress = progress });

                var trifoldAchievementResponse = await StudentAchievementService.CheckTrifold(new(session.Id, progress));
                if (trifoldAchievementResponse.Ok)
                    await Notify(session.Id, session.User.Contact.Id, new StudentAchievementAdded { StudentAchievement = trifoldAchievementResponse.Value });

                var bookProgress = await BookProgressService.Fetch(new(session.Id, new() { Id = progress.BookId }));
                await Notify(session.Id, session.User.Contact.Id, new BookProgressUpdated { Progress = bookProgress });
            }

            return response;
        });
    }
}