namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<User?>> AccountSettingsFetch(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AccountSettingsService.Fetch(request));
    }

    public async Task<Response> AccountSettingsSave(Request<User> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
        {
            request.Value.Id = session.User!.Id;
            return await AccountSettingsService.Save(request);
        });
    }
}