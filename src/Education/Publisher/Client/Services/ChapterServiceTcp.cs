namespace Crudspa.Education.Publisher.Client.Services;

public class ChapterServiceTcp(IProxyWrappers proxyWrappers) : IChapterService
{
    public async Task<Response<IList<Chapter>>> FetchForBook(Request<Book> request) =>
        await proxyWrappers.Send<IList<Chapter>>("ChapterFetchForBook", request);

    public async Task<Response<Chapter?>> Fetch(Request<Chapter> request) =>
        await proxyWrappers.Send<Chapter?>("ChapterFetch", request);

    public async Task<Response<Chapter?>> Add(Request<Chapter> request) =>
        await proxyWrappers.Send<Chapter?>("ChapterAdd", request);

    public async Task<Response> Save(Request<Chapter> request) =>
        await proxyWrappers.Send("ChapterSave", request);

    public async Task<Response> Remove(Request<Chapter> request) =>
        await proxyWrappers.Send("ChapterRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Chapter>> request) =>
        await proxyWrappers.Send("ChapterSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ChapterFetchContentStatusNames", request);

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ChapterFetchBinderTypeNames", request);

    public async Task<Response<IList<Page>>> FetchPages(Request<ChapterPage> request) =>
        await proxyWrappers.Send<IList<Page>>("ChapterFetchPages", request);

    public async Task<Response<Page?>> FetchPage(Request<ChapterPage> request) =>
        await proxyWrappers.Send<Page?>("ChapterFetchPage", request);

    public async Task<Response<Page?>> AddPage(Request<ChapterPage> request) =>
        await proxyWrappers.Send<Page?>("ChapterAddPage", request);

    public async Task<Response> SavePage(Request<ChapterPage> request) =>
        await proxyWrappers.Send("ChapterSavePage", request);

    public async Task<Response> RemovePage(Request<ChapterPage> request) =>
        await proxyWrappers.Send("ChapterRemovePage", request);

    public async Task<Response> SavePageOrder(Request<ChapterPage> request) =>
        await proxyWrappers.Send("ChapterSavePageOrder", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<ChapterSection> request) =>
        await proxyWrappers.Send<IList<Section>>("ChapterFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<ChapterSection> request) =>
        await proxyWrappers.Send<Section?>("ChapterFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<ChapterSection> request) =>
        await proxyWrappers.Send<Section?>("ChapterAddSection", request);

    public async Task<Response<Section?>> DuplicateSection(Request<ChapterSection> request) =>
        await proxyWrappers.Send<Section?>("ChapterDuplicateSection", request);

    public async Task<Response> SaveSection(Request<ChapterSection> request) =>
        await proxyWrappers.Send("ChapterSaveSection", request);

    public async Task<Response> RemoveSection(Request<ChapterSection> request) =>
        await proxyWrappers.Send("ChapterRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<ChapterSection> request) =>
        await proxyWrappers.Send("ChapterSaveSectionOrder", request);
}