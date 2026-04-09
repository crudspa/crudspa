namespace Crudspa.Education.Publisher.Client.Services;

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

    public async Task<Response<Book>> FetchRelations(Request<Book> request) =>
        await proxyWrappers.Send<Book>("BookFetchRelations", request);

    public async Task<Response> SaveRelations(Request<Book> request) =>
        await proxyWrappers.Send("BookSaveRelations", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("BookFetchContentStatusNames", request);

    public async Task<Response<IList<Orderable>>> FetchBookSeasonNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("BookFetchBookSeasonNames", request);

    public async Task<Response<IList<Orderable>>> FetchBookCategoryNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("BookFetchBookCategoryNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("BookFetchAchievementNames", request);

    public async Task<Response<Book?>> FetchPrefaceBinderId(Request<Book> request) =>
        await proxyWrappers.Send<Book?>("BookFetchPrefaceBinderId", request);

    public async Task<Response<IList<Page>>> FetchPages(Request<BookPage> request) =>
        await proxyWrappers.Send<IList<Page>>("BookFetchPages", request);

    public async Task<Response<Page?>> FetchPage(Request<BookPage> request) =>
        await proxyWrappers.Send<Page?>("BookFetchPage", request);

    public async Task<Response<Page?>> AddPage(Request<BookPage> request) =>
        await proxyWrappers.Send<Page?>("BookAddPage", request);

    public async Task<Response> SavePage(Request<BookPage> request) =>
        await proxyWrappers.Send("BookSavePage", request);

    public async Task<Response> RemovePage(Request<BookPage> request) =>
        await proxyWrappers.Send("BookRemovePage", request);

    public async Task<Response> SavePageOrder(Request<BookPage> request) =>
        await proxyWrappers.Send("BookSavePageOrder", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<BookSection> request) =>
        await proxyWrappers.Send<IList<Section>>("BookFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<BookSection> request) =>
        await proxyWrappers.Send<Section?>("BookFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<BookSection> request) =>
        await proxyWrappers.Send<Section?>("BookAddSection", request);

    public async Task<Response> SaveSection(Request<BookSection> request) =>
        await proxyWrappers.Send("BookSaveSection", request);

    public async Task<Response> RemoveSection(Request<BookSection> request) =>
        await proxyWrappers.Send("BookRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<BookSection> request) =>
        await proxyWrappers.Send("BookSaveSectionOrder", request);
}