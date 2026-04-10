namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IPanePageService
{
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Page>>> FetchPages(Request<PageForPane> request);
    Task<Response<Page?>> FetchPage(Request<PageForPane> request);
    Task<Response<Page?>> AddPage(Request<PageForPane> request);
    Task<Response> SavePage(Request<PageForPane> request);
    Task<Response> RemovePage(Request<PageForPane> request);
    Task<Response> SavePageOrder(Request<PageForPane> request);
    Task<Response<IList<Section>>> FetchSections(Request<SectionForPane> request);
    Task<Response<Section?>> FetchSection(Request<SectionForPane> request);
    Task<Response<Section?>> AddSection(Request<SectionForPane> request);
    Task<Response<Section?>> DuplicateSection(Request<SectionForPane> request);
    Task<Response> SaveSection(Request<SectionForPane> request);
    Task<Response> RemoveSection(Request<SectionForPane> request);
    Task<Response> SaveSectionOrder(Request<SectionForPane> request);
    Task<Response<Binder?>> AddBinder(Request<BinderForPane> request);
}