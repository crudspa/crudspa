namespace Crudspa.Education.Publisher.Client.Services;

public class LessonServiceTcp(IProxyWrappers proxyWrappers) : ILessonService
{
    public async Task<Response<IList<Lesson>>> FetchForUnit(Request<Unit> request) =>
        await proxyWrappers.Send<IList<Lesson>>("LessonFetchForUnit", request);

    public async Task<Response<Lesson?>> Fetch(Request<Lesson> request) =>
        await proxyWrappers.Send<Lesson?>("LessonFetch", request);

    public async Task<Response<Lesson?>> Add(Request<Lesson> request) =>
        await proxyWrappers.Send<Lesson?>("LessonAdd", request);

    public async Task<Response> Save(Request<Lesson> request) =>
        await proxyWrappers.Send("LessonSave", request);

    public async Task<Response> Remove(Request<Lesson> request) =>
        await proxyWrappers.Send("LessonRemove", request);

    public async Task<Response<Copy>> Copy(Request<Copy> request) =>
        await proxyWrappers.Send<Copy>("LessonCopy", request);

    public async Task<Response> SaveOrder(Request<IList<Lesson>> request) =>
        await proxyWrappers.Send("LessonSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("LessonFetchContentStatusNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("LessonFetchAchievementNames", request);
}