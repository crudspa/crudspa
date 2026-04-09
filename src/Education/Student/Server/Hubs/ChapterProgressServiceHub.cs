namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response<IList<ChapterProgress>>> ChapterProgressFetchAll(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ChapterProgressService.FetchAll(request));
    }

    public async Task<Response> ChapterProgressAddCompleted(Request<ChapterCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await ChapterProgressService.AddCompleted(request);

            if (response.Ok)
            {
                var progress = await ChapterProgressService.Fetch(new(session.Id, new() { Id = request.Value.ChapterId }));
                await Notify(session.Id, session.User.Contact.Id, new ChapterProgressUpdated { Progress = progress });

                var bookProgress = await BookProgressService.Fetch(new(session.Id, new() { Id = progress.BookId }));
                await Notify(session.Id, session.User.Contact.Id, new BookProgressUpdated { Progress = bookProgress });
            }

            return response;
        });
    }
}