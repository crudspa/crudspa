namespace Crudspa.Samples.Catalog.Client.Services;

public class BookServiceTcp(IProxyWrappers proxyWrappers) : IBookService
{
    public async Task<Response<IList<Book>>> Search(Request<BookSearch> request) =>
        await proxyWrappers.Send<IList<Book>>("BookSearch", request);

    public async Task<Response<Book?>> Fetch(Request<Book> request) =>
        await proxyWrappers.Send<Book?>("BookFetch", request);

    public async Task<Response<Book?>> Add(Request<Book> request) =>
        await proxyWrappers.Send<Book?>("BookAdd", request);

    public async Task<Response> Save(Request<Book> request) =>
        await proxyWrappers.Send("BookSave", request);

    public async Task<Response> Remove(Request<Book> request) =>
        await proxyWrappers.Send("BookRemove", request);

    public async Task<Response<IList<Orderable>>> FetchGenreNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("BookFetchGenreNames", request);

    public async Task<Response<IList<Orderable>>> FetchTagNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("BookFetchTagNames", request);

    public async Task<Response<IList<Orderable>>> FetchFormatNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("BookFetchFormatNames", request);
}