namespace Crudspa.Content.Design.Client.Services;

public class PanePageServiceTcp(IProxyWrappers proxyWrappers) : IPanePageService
{
    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PanePageFetchContentStatusNames", request);

    public async Task<Response<IList<Page>>> FetchPages(Request<PageForPane> request) =>
        await proxyWrappers.Send<IList<Page>>("PanePageFetchPages", request);

    public async Task<Response<Page?>> FetchPage(Request<PageForPane> request) =>
        await proxyWrappers.Send<Page?>("PanePageFetchPage", request);

    public async Task<Response<Page?>> AddPage(Request<PageForPane> request) =>
        await proxyWrappers.Send<Page?>("PanePageAddPage", request);

    public async Task<Response> SavePage(Request<PageForPane> request) =>
        await proxyWrappers.Send("PanePageSavePage", request);

    public async Task<Response> RemovePage(Request<PageForPane> request) =>
        await proxyWrappers.Send("PanePageRemovePage", request);

    public async Task<Response> SavePageOrder(Request<PageForPane> request) =>
        await proxyWrappers.Send("PanePageSavePageOrder", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<SectionForPane> request) =>
        await proxyWrappers.Send<IList<Section>>("PanePageFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<SectionForPane> request) =>
        await proxyWrappers.Send<Section?>("PanePageFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<SectionForPane> request) =>
        await proxyWrappers.Send<Section?>("PanePageAddSection", request);

    public async Task<Response<Section?>> DuplicateSection(Request<SectionForPane> request) =>
        await proxyWrappers.Send<Section?>("PanePageDuplicateSection", request);

    public async Task<Response> SaveSection(Request<SectionForPane> request) =>
        await proxyWrappers.Send("PanePageSaveSection", request);

    public async Task<Response> RemoveSection(Request<SectionForPane> request) =>
        await proxyWrappers.Send("PanePageRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<SectionForPane> request) =>
        await proxyWrappers.Send("PanePageSaveSectionOrder", request);

    public async Task<Response<Binder?>> AddBinder(Request<BinderForPane> request) =>
        await proxyWrappers.Send<Binder?>("PanePageAddBinder", request);
}