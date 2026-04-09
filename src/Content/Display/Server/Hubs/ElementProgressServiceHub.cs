namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<IList<ElementProgress>>> ElementProgressFetchAll(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ElementProgressService.FetchAll(request));
    }

    public async Task<Response> ElementProgressAddCompleted(Request<ElementCompleted> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            var response = await ElementProgressService.AddCompleted(request);

            if (response.Ok)
            {
                var progress = await ElementProgressService.Fetch(new(session.Id, new() { Id = request.Value.ElementId }));
                await Notify(session.Id, session.User.Contact.Id, new ElementProgressUpdated { Progress = progress });
            }

            return response;
        });
    }

    public async Task<Response> ElementProgressAddLink(Request<ElementLink> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ElementProgressService.AddLink(request));
    }
}