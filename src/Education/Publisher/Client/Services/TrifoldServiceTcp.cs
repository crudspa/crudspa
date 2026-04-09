namespace Crudspa.Education.Publisher.Client.Services;

public class TrifoldServiceTcp(IProxyWrappers proxyWrappers) : ITrifoldService
{
    public async Task<Response<IList<Trifold>>> FetchForBook(Request<Book> request) =>
        await proxyWrappers.Send<IList<Trifold>>("TrifoldFetchForBook", request);

    public async Task<Response<Trifold?>> Fetch(Request<Trifold> request) =>
        await proxyWrappers.Send<Trifold?>("TrifoldFetch", request);

    public async Task<Response<Trifold?>> Add(Request<Trifold> request) =>
        await proxyWrappers.Send<Trifold?>("TrifoldAdd", request);

    public async Task<Response> Save(Request<Trifold> request) =>
        await proxyWrappers.Send("TrifoldSave", request);

    public async Task<Response> Remove(Request<Trifold> request) =>
        await proxyWrappers.Send("TrifoldRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Trifold>> request) =>
        await proxyWrappers.Send("TrifoldSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("TrifoldFetchContentStatusNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("TrifoldFetchAchievementNames", request);

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("TrifoldFetchBinderTypeNames", request);

    public async Task<Response<IList<Page>>> FetchPages(Request<TrifoldPage> request) =>
        await proxyWrappers.Send<IList<Page>>("TrifoldFetchPages", request);

    public async Task<Response<Page?>> FetchPage(Request<TrifoldPage> request) =>
        await proxyWrappers.Send<Page?>("TrifoldFetchPage", request);

    public async Task<Response<Page?>> AddPage(Request<TrifoldPage> request) =>
        await proxyWrappers.Send<Page?>("TrifoldAddPage", request);

    public async Task<Response> SavePage(Request<TrifoldPage> request) =>
        await proxyWrappers.Send("TrifoldSavePage", request);

    public async Task<Response> RemovePage(Request<TrifoldPage> request) =>
        await proxyWrappers.Send("TrifoldRemovePage", request);

    public async Task<Response> SavePageOrder(Request<TrifoldPage> request) =>
        await proxyWrappers.Send("TrifoldSavePageOrder", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<TrifoldSection> request) =>
        await proxyWrappers.Send<IList<Section>>("TrifoldFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<TrifoldSection> request) =>
        await proxyWrappers.Send<Section?>("TrifoldFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<TrifoldSection> request) =>
        await proxyWrappers.Send<Section?>("TrifoldAddSection", request);

    public async Task<Response> SaveSection(Request<TrifoldSection> request) =>
        await proxyWrappers.Send("TrifoldSaveSection", request);

    public async Task<Response> RemoveSection(Request<TrifoldSection> request) =>
        await proxyWrappers.Send("TrifoldRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<TrifoldSection> request) =>
        await proxyWrappers.Send("TrifoldSaveSectionOrder", request);
}