namespace Crudspa.Content.Design.Client.Services;

public class TrackServiceTcp(IProxyWrappers proxyWrappers) : ITrackService
{
    public async Task<Response<IList<Track>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Track>>("TrackFetchForPortal", request);

    public async Task<Response<Track?>> Fetch(Request<Track> request) =>
        await proxyWrappers.Send<Track?>("TrackFetch", request);

    public async Task<Response<Track?>> Add(Request<Track> request) =>
        await proxyWrappers.Send<Track?>("TrackAdd", request);

    public async Task<Response> Save(Request<Track> request) =>
        await proxyWrappers.Send("TrackSave", request);

    public async Task<Response> Remove(Request<Track> request) =>
        await proxyWrappers.Send("TrackRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Track>> request) =>
        await proxyWrappers.Send("TrackSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("TrackFetchContentStatusNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Named>>("TrackFetchAchievementNames", request);
}