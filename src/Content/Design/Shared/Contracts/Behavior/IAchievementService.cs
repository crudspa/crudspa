namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IAchievementService
{
    Task<Response<IList<Achievement>>> FetchForPortal(Request<Portal> request);
    Task<Response<Achievement?>> Fetch(Request<Achievement> request);
    Task<Response<Achievement?>> Add(Request<Achievement> request);
    Task<Response> Save(Request<Achievement> request);
    Task<Response> Remove(Request<Achievement> request);
}