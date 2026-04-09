namespace Crudspa.Education.Publisher.Client.Services;

public class GameActivityServiceTcp(IProxyWrappers proxyWrappers) : IGameActivityService
{
    public async Task<Response<IList<GameActivity>>> FetchForSection(Request<GameSection> request) =>
        await proxyWrappers.Send<IList<GameActivity>>("GameActivityFetchForSection", request);

    public async Task<Response<GameActivity?>> Fetch(Request<GameActivity> request) =>
        await proxyWrappers.Send<GameActivity?>("GameActivityFetch", request);

    public async Task<Response<GameActivity?>> Add(Request<GameActivity> request) =>
        await proxyWrappers.Send<GameActivity?>("GameActivityAdd", request);

    public async Task<Response> Save(Request<GameActivity> request) =>
        await proxyWrappers.Send("GameActivitySave", request);

    public async Task<Response> Remove(Request<GameActivity> request) =>
        await proxyWrappers.Send("GameActivityRemove", request);

    public async Task<Response> SaveOrder(Request<IList<GameActivity>> request) =>
        await proxyWrappers.Send("GameActivitySaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchGameActivityTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("GameActivityFetchGameActivityTypeNames", request);

    public async Task<Response<IList<ActivityTypeFull>>> FetchActivityTypes(Request request) =>
        await proxyWrappers.SendAndCache<IList<ActivityTypeFull>>("GameActivityFetchActivityTypes", request);

    public async Task<Response<IList<Named>>> FetchContentAreaNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Named>>("GameActivityFetchContentAreaNames", request);

    public async Task<Response<IList<Orderable>>> FetchResearchGroupNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("GameActivityFetchResearchGroupNames", request);

    public async Task<Response<IList<Named>>> FetchBooks(Request request) =>
        await proxyWrappers.Send<IList<Named>>("GameActivityFetchBooks", request);

    public async Task<Response<IList<Named>>> FetchSections(Request<Book> request) =>
        await proxyWrappers.Send<IList<Named>>("GameActivityFetchSections", request);

    public async Task<Response> Share(Request<GameActivityShare> request) =>
        await proxyWrappers.Send("GameActivityShare", request);
}