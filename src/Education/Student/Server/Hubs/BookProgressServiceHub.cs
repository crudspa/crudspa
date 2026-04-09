namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response<IList<BookProgress>>> BookProgressFetchAll(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await BookProgressService.FetchAll(request));
    }

    public async Task<Response> BookProgressAddPrefaceCompleted(Request<PrefaceCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await BookProgressService.AddPrefaceCompleted(request);

            if (response.Ok)
            {
                var progress = await BookProgressService.Fetch(new(session.Id, new() { Id = request.Value.BookId }));
                await Notify(session.Id, session.User.Contact.Id, new BookProgressUpdated { Progress = progress });
            }

            return response;
        });
    }
}