namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response> LinkClickAdd(Request<LinkClick> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await LinkClickService.Add(request));
    }
}