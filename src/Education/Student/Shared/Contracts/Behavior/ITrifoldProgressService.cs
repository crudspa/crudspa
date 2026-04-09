namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface ITrifoldProgressService
{
    Task<Response<IList<TrifoldProgress>>> FetchAll(Request request);
    Task<TrifoldProgress> Fetch(Request<Trifold> request);
    Task<Response> AddCompleted(Request<TrifoldCompleted> request);
}