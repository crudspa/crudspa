namespace Crudspa.Samples.Catalog.Shared.Contracts.Behavior;

public interface IBookService
{
    Task<Response<IList<Book>>> Search(Request<BookSearch> request);
    Task<Response<Book?>> Fetch(Request<Book> request);
    Task<Response<Book?>> Add(Request<Book> request);
    Task<Response> Save(Request<Book> request);
    Task<Response> Remove(Request<Book> request);
    Task<Response<IList<Orderable>>> FetchGenreNames(Request request);
    Task<Response<IList<Orderable>>> FetchTagNames(Request request);
    Task<Response<IList<Orderable>>> FetchFormatNames(Request request);
}