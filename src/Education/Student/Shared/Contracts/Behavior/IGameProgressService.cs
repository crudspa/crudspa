namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IGameProgressService
{
    Task<Response<IList<GameProgress>>> FetchAll(Request request);
    Task<GameProgress> Fetch(Request<Game> request);
    Task<Response> AddCompleted(Request<GameCompleted> request);
}