namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response> MediaPlayAdd(Request<MediaPlay> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await MediaPlayService.Add(request));
    }
}