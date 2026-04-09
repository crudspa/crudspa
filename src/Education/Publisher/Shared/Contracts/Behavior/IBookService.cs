namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IBookService
{
    Task<Response<IList<Book>>> Search(Request<BookSearch> request);
    Task<Response<Book?>> Fetch(Request<Book> request);
    Task<Response<Book?>> Add(Request<Book> request);
    Task<Response> Save(Request<Book> request);
    Task<Response> Remove(Request<Book> request);
    Task<Response<Book>> FetchRelations(Request<Book> request);
    Task<Response> SaveRelations(Request<Book> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Orderable>>> FetchBookSeasonNames(Request request);
    Task<Response<IList<Orderable>>> FetchBookCategoryNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request request);
    Task<Response<Book?>> FetchPrefaceBinderId(Request<Book> request);
    Task<Response<IList<Page>>> FetchPages(Request<BookPage> request);
    Task<Response<Page?>> FetchPage(Request<BookPage> request);
    Task<Response<Page?>> AddPage(Request<BookPage> request);
    Task<Response> SavePage(Request<BookPage> request);
    Task<Response> RemovePage(Request<BookPage> request);
    Task<Response> SavePageOrder(Request<BookPage> request);
    Task<Response<IList<Section>>> FetchSections(Request<BookSection> request);
    Task<Response<Section?>> FetchSection(Request<BookSection> request);
    Task<Response<Section?>> AddSection(Request<BookSection> request);
    Task<Response> SaveSection(Request<BookSection> request);
    Task<Response> RemoveSection(Request<BookSection> request);
    Task<Response> SaveSectionOrder(Request<BookSection> request);
}