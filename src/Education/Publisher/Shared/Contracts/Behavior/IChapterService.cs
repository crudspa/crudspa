namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IChapterService
{
    Task<Response<IList<Chapter>>> FetchForBook(Request<Book> request);
    Task<Response<Chapter?>> Fetch(Request<Chapter> request);
    Task<Response<Chapter?>> Add(Request<Chapter> request);
    Task<Response> Save(Request<Chapter> request);
    Task<Response> Remove(Request<Chapter> request);
    Task<Response> SaveOrder(Request<IList<Chapter>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request);
    Task<Response<IList<Page>>> FetchPages(Request<ChapterPage> request);
    Task<Response<Page?>> FetchPage(Request<ChapterPage> request);
    Task<Response<Page?>> AddPage(Request<ChapterPage> request);
    Task<Response> SavePage(Request<ChapterPage> request);
    Task<Response> RemovePage(Request<ChapterPage> request);
    Task<Response> SavePageOrder(Request<ChapterPage> request);
    Task<Response<IList<Section>>> FetchSections(Request<ChapterSection> request);
    Task<Response<Section?>> FetchSection(Request<ChapterSection> request);
    Task<Response<Section?>> AddSection(Request<ChapterSection> request);
    Task<Response> SaveSection(Request<ChapterSection> request);
    Task<Response> RemoveSection(Request<ChapterSection> request);
    Task<Response> SaveSectionOrder(Request<ChapterSection> request);
}