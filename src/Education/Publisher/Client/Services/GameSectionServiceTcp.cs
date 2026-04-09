namespace Crudspa.Education.Publisher.Client.Services;

public class GameSectionServiceTcp(IProxyWrappers proxyWrappers) : IGameSectionService
{
    public async Task<Response<IList<GameSection>>> FetchForGame(Request<Game> request) =>
        await proxyWrappers.Send<IList<GameSection>>("GameSectionFetchForGame", request);

    public async Task<Response<GameSection?>> Fetch(Request<GameSection> request) =>
        await proxyWrappers.Send<GameSection?>("GameSectionFetch", request);

    public async Task<Response<GameSection?>> Add(Request<GameSection> request) =>
        await proxyWrappers.Send<GameSection?>("GameSectionAdd", request);

    public async Task<Response> Save(Request<GameSection> request) =>
        await proxyWrappers.Send("GameSectionSave", request);

    public async Task<Response> Remove(Request<GameSection> request) =>
        await proxyWrappers.Send("GameSectionRemove", request);

    public async Task<Response> SaveOrder(Request<IList<GameSection>> request) =>
        await proxyWrappers.Send("GameSectionSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("GameSectionFetchContentStatusNames", request);

    public async Task<Response<IList<Orderable>>> FetchGameSectionTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("GameSectionFetchGameSectionTypeNames", request);
}