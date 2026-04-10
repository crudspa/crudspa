namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ICourseService
{
    Task<Response<IList<Course>>> FetchForTrack(Request<Track> request);
    Task<Response<Course?>> Fetch(Request<Course> request);
    Task<Response<Course?>> Add(Request<Course> request);
    Task<Response> Save(Request<Course> request);
    Task<Response> Remove(Request<Course> request);
    Task<Response> SaveOrder(Request<IList<Course>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request<Portal> request);
    Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request);
    Task<Response<IList<Page>>> FetchPages(Request<CoursePage> request);
    Task<Response<Page?>> FetchPage(Request<CoursePage> request);
    Task<Response<Page?>> AddPage(Request<CoursePage> request);
    Task<Response> SavePage(Request<CoursePage> request);
    Task<Response> RemovePage(Request<CoursePage> request);
    Task<Response> SavePageOrder(Request<CoursePage> request);
    Task<Response<IList<Section>>> FetchSections(Request<CourseSection> request);
    Task<Response<Section?>> FetchSection(Request<CourseSection> request);
    Task<Response<Section?>> AddSection(Request<CourseSection> request);
    Task<Response<Section?>> DuplicateSection(Request<CourseSection> request);
    Task<Response> SaveSection(Request<CourseSection> request);
    Task<Response> RemoveSection(Request<CourseSection> request);
    Task<Response> SaveSectionOrder(Request<CourseSection> request);
}