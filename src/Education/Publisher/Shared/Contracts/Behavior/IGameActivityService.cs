namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IGameActivityService
{
    Task<Response<IList<GameActivity>>> FetchForSection(Request<GameSection> request);
    Task<Response<GameActivity?>> Fetch(Request<GameActivity> request);
    Task<Response<GameActivity?>> Add(Request<GameActivity> request);
    Task<Response> Save(Request<GameActivity> request);
    Task<Response> Remove(Request<GameActivity> request);
    Task<Response> SaveOrder(Request<IList<GameActivity>> request);
    Task<Response<IList<ActivityTypeFull>>> FetchActivityTypes(Request request);
    Task<Response<IList<Named>>> FetchContentAreaNames(Request request);
    Task<Response<IList<Orderable>>> FetchResearchGroupNames(Request request);
    Task<Response<IList<Orderable>>> FetchGameActivityTypeNames(Request request);
    Task<Response<IList<Named>>> FetchBooks(Request request);
    Task<Response<IList<Named>>> FetchSections(Request<Book> request);
    Task<Response> Share(Request<GameActivityShare> request);
}