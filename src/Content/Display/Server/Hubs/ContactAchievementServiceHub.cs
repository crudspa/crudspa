namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<IList<ContactAchievement>>> ContactAchievementFetchAchievements(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ContactAchievementService.FetchAchievements(request));
    }

    public async Task<Response<ContactAchievement?>> ContactAchievementFetchAchievement(Request<ContactAchievement> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ContactAchievementService.FetchAchievement(request));
    }
}