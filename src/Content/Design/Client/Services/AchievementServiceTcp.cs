namespace Crudspa.Content.Design.Client.Services;

public class AchievementServiceTcp(IProxyWrappers proxyWrappers) : IAchievementService
{
    public async Task<Response<IList<Achievement>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Achievement>>("AchievementFetchForPortal", request);

    public async Task<Response<Achievement?>> Fetch(Request<Achievement> request) =>
        await proxyWrappers.Send<Achievement?>("AchievementFetch", request);

    public async Task<Response<Achievement?>> Add(Request<Achievement> request) =>
        await proxyWrappers.Send<Achievement?>("AchievementAdd", request);

    public async Task<Response> Save(Request<Achievement> request) =>
        await proxyWrappers.Send("AchievementSave", request);

    public async Task<Response> Remove(Request<Achievement> request) =>
        await proxyWrappers.Send("AchievementRemove", request);
}