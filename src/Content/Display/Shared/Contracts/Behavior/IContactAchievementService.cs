namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IContactAchievementService
{
    Task<Response<IList<ContactAchievement>>> FetchAchievements(Request request);
    Task<Response<ContactAchievement?>> FetchAchievement(Request<ContactAchievement> request);
    Task<Response<ContactAchievement?>> CheckCourse(Request<CourseProgress> request);
    Task<Response<ContactAchievement?>> CheckTrack(Request<CourseProgress> request);
}