using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Token>>> TokenFetchForMembership(Request<Membership> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await TokenService.FetchForMembership(request));
    }
}