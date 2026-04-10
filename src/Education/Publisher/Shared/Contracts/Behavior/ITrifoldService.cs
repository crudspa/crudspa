namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface ITrifoldService
{
    Task<Response<IList<Trifold>>> FetchForBook(Request<Book> request);
    Task<Response<Trifold?>> Fetch(Request<Trifold> request);
    Task<Response<Trifold?>> Add(Request<Trifold> request);
    Task<Response> Save(Request<Trifold> request);
    Task<Response> Remove(Request<Trifold> request);
    Task<Response> SaveOrder(Request<IList<Trifold>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request request);
    Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request);
    Task<Response<IList<Page>>> FetchPages(Request<TrifoldPage> request);
    Task<Response<Page?>> FetchPage(Request<TrifoldPage> request);
    Task<Response<Page?>> AddPage(Request<TrifoldPage> request);
    Task<Response> SavePage(Request<TrifoldPage> request);
    Task<Response> RemovePage(Request<TrifoldPage> request);
    Task<Response> SavePageOrder(Request<TrifoldPage> request);
    Task<Response<IList<Section>>> FetchSections(Request<TrifoldSection> request);
    Task<Response<Section?>> FetchSection(Request<TrifoldSection> request);
    Task<Response<Section?>> AddSection(Request<TrifoldSection> request);
    Task<Response<Section?>> DuplicateSection(Request<TrifoldSection> request);
    Task<Response> SaveSection(Request<TrifoldSection> request);
    Task<Response> RemoveSection(Request<TrifoldSection> request);
    Task<Response> SaveSectionOrder(Request<TrifoldSection> request);
}