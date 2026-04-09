namespace Crudspa.Education.Publisher.Client.Services;

public class GameServiceTcp(IProxyWrappers proxyWrappers) : IGameService
{
    public async Task<Response<IList<Game>>> SearchForBook(Request<GameSearch> request) =>
        await proxyWrappers.Send<IList<Game>>("GameSearchForBook", request);

    public async Task<Response<Game?>> Fetch(Request<Game> request) =>
        await proxyWrappers.Send<Game?>("GameFetch", request);

    public async Task<Response<Game?>> Add(Request<Game> request) =>
        await proxyWrappers.Send<Game?>("GameAdd", request);

    public async Task<Response> Save(Request<Game> request) =>
        await proxyWrappers.Send("GameSave", request);

    public async Task<Response> Remove(Request<Game> request) =>
        await proxyWrappers.Send("GameRemove", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("GameFetchContentStatusNames", request);

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request) =>
        await proxyWrappers.SendAndCache<IList<IconFull>>("GameFetchIcons", request);

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("GameFetchGradeNames", request);

    public async Task<Response<IList<Orderable>>> FetchAssessmentLevelNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("GameFetchAssessmentLevelNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("GameFetchAchievementNames", request);
}