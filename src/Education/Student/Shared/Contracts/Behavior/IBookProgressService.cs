namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IBookProgressService
{
    Task<Response<IList<BookProgress>>> FetchAll(Request request);
    Task<BookProgress> Fetch(Request<Book> request);
    Task<Response> AddPrefaceCompleted(Request<PrefaceCompleted> request);
}