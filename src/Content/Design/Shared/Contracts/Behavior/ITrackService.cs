namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ITrackService
{
    Task<Response<IList<Track>>> FetchForPortal(Request<Portal> request);
    Task<Response<Track?>> Fetch(Request<Track> request);
    Task<Response<Track?>> Add(Request<Track> request);
    Task<Response> Save(Request<Track> request);
    Task<Response> Remove(Request<Track> request);
    Task<Response> SaveOrder(Request<IList<Track>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request<Portal> request);
}