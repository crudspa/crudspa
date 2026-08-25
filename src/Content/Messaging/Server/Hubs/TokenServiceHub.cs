namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<IList<Token>>> TokenFetchForMembership(Request<Membership> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await TokenService.FetchForMembership(request));
    }

    public async Task<Response<IList<Token>>> TokenFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await TokenService.FetchForPortal(request));
    }
}