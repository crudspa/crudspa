namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface ICourseRunService
{
    Task<Response<Course?>> FetchCourse(Request<Course> request);
    Task<Response<Track?>> FetchTrack(Request<Track> request);
    Task<Response<PortalTracks?>> FetchPortalTracks(Request request);
    Task<Response<IList<CourseProgress>>> FetchAllProgress(Request request);
    Task<CourseProgress> FetchProgress(Request<Course> request);
    Task<Response> AddCompleted(Request<CourseCompleted> request);
}