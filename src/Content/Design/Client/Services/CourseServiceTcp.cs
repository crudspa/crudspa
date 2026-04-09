namespace Crudspa.Content.Design.Client.Services;

public class CourseServiceTcp(IProxyWrappers proxyWrappers) : ICourseService
{
    public async Task<Response<IList<Course>>> FetchForTrack(Request<Track> request) =>
        await proxyWrappers.Send<IList<Course>>("CourseFetchForTrack", request);

    public async Task<Response<Course?>> Fetch(Request<Course> request) =>
        await proxyWrappers.Send<Course?>("CourseFetch", request);

    public async Task<Response<Course?>> Add(Request<Course> request) =>
        await proxyWrappers.Send<Course?>("CourseAdd", request);

    public async Task<Response> Save(Request<Course> request) =>
        await proxyWrappers.Send("CourseSave", request);

    public async Task<Response> Remove(Request<Course> request) =>
        await proxyWrappers.Send("CourseRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Course>> request) =>
        await proxyWrappers.Send("CourseSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("CourseFetchContentStatusNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Named>>("CourseFetchAchievementNames", request);

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("CourseFetchBinderTypeNames", request);

    public async Task<Response<IList<Page>>> FetchPages(Request<CoursePage> request) =>
        await proxyWrappers.Send<IList<Page>>("CourseFetchPages", request);

    public async Task<Response<Page?>> FetchPage(Request<CoursePage> request) =>
        await proxyWrappers.Send<Page?>("CourseFetchPage", request);

    public async Task<Response<Page?>> AddPage(Request<CoursePage> request) =>
        await proxyWrappers.Send<Page?>("CourseAddPage", request);

    public async Task<Response> SavePage(Request<CoursePage> request) =>
        await proxyWrappers.Send("CourseSavePage", request);

    public async Task<Response> RemovePage(Request<CoursePage> request) =>
        await proxyWrappers.Send("CourseRemovePage", request);

    public async Task<Response> SavePageOrder(Request<CoursePage> request) =>
        await proxyWrappers.Send("CourseSavePageOrder", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<CourseSection> request) =>
        await proxyWrappers.Send<IList<Section>>("CourseFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<CourseSection> request) =>
        await proxyWrappers.Send<Section?>("CourseFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<CourseSection> request) =>
        await proxyWrappers.Send<Section?>("CourseAddSection", request);

    public async Task<Response> SaveSection(Request<CourseSection> request) =>
        await proxyWrappers.Send("CourseSaveSection", request);

    public async Task<Response> RemoveSection(Request<CourseSection> request) =>
        await proxyWrappers.Send("CourseRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<CourseSection> request) =>
        await proxyWrappers.Send("CourseSaveSectionOrder", request);
}