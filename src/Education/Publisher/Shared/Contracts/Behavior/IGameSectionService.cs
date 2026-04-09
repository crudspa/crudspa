namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IGameSectionService
{
    Task<Response<IList<GameSection>>> FetchForGame(Request<Game> request);
    Task<Response<GameSection?>> Fetch(Request<GameSection> request);
    Task<Response<GameSection?>> Add(Request<GameSection> request);
    Task<Response> Save(Request<GameSection> request);
    Task<Response> Remove(Request<GameSection> request);
    Task<Response> SaveOrder(Request<IList<GameSection>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Orderable>>> FetchGameSectionTypeNames(Request request);
}