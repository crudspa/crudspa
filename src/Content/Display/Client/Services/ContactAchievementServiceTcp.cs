namespace Crudspa.Content.Display.Client.Services;

public class ContactAchievementServiceTcp(IProxyWrappers proxyWrappers) : IContactAchievementService
{
    public async Task<Response<IList<ContactAchievement>>> FetchAchievements(Request request) =>
        await proxyWrappers.Send<IList<ContactAchievement>>("ContactAchievementFetchAchievements", request);

    public async Task<Response<ContactAchievement?>> FetchAchievement(Request<ContactAchievement> request) =>
        await proxyWrappers.Send<ContactAchievement?>("ContactAchievementFetchAchievement", request);

    public Task<Response<ContactAchievement?>> CheckCourse(Request<CourseProgress> request) =>
        throw new NotImplementedException("Implemented server side only.");

    public Task<Response<ContactAchievement?>> CheckTrack(Request<CourseProgress> request) =>
        throw new NotImplementedException("Implemented server side only.");
}