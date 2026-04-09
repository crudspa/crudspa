namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<IList<Named>>> AddressFetchUsaStateNames(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await AddressService.FetchUsaStateNames(request));
    }
}