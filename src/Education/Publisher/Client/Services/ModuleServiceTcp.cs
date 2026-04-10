namespace Crudspa.Education.Publisher.Client.Services;

public class ModuleServiceTcp(IProxyWrappers proxyWrappers) : IModuleService
{
    public async Task<Response<IList<Module>>> FetchForBook(Request<Book> request) =>
        await proxyWrappers.Send<IList<Module>>("ModuleFetchForBook", request);

    public async Task<Response<Module?>> Fetch(Request<Module> request) =>
        await proxyWrappers.Send<Module?>("ModuleFetch", request);

    public async Task<Response<Module?>> Add(Request<Module> request) =>
        await proxyWrappers.Send<Module?>("ModuleAdd", request);

    public async Task<Response> Save(Request<Module> request) =>
        await proxyWrappers.Send("ModuleSave", request);

    public async Task<Response> Remove(Request<Module> request) =>
        await proxyWrappers.Send("ModuleRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Module>> request) =>
        await proxyWrappers.Send("ModuleSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ModuleFetchContentStatusNames", request);

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request) =>
        await proxyWrappers.SendAndCache<IList<IconFull>>("ModuleFetchIcons", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("ModuleFetchAchievementNames", request);

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ModuleFetchBinderTypeNames", request);

    public async Task<Response<IList<Page>>> FetchPages(Request<ModulePage> request) =>
        await proxyWrappers.Send<IList<Page>>("ModuleFetchPages", request);

    public async Task<Response<Page?>> FetchPage(Request<ModulePage> request) =>
        await proxyWrappers.Send<Page?>("ModuleFetchPage", request);

    public async Task<Response<Page?>> AddPage(Request<ModulePage> request) =>
        await proxyWrappers.Send<Page?>("ModuleAddPage", request);

    public async Task<Response> SavePage(Request<ModulePage> request) =>
        await proxyWrappers.Send("ModuleSavePage", request);

    public async Task<Response> RemovePage(Request<ModulePage> request) =>
        await proxyWrappers.Send("ModuleRemovePage", request);

    public async Task<Response> SavePageOrder(Request<ModulePage> request) =>
        await proxyWrappers.Send("ModuleSavePageOrder", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<ModuleSection> request) =>
        await proxyWrappers.Send<IList<Section>>("ModuleFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<ModuleSection> request) =>
        await proxyWrappers.Send<Section?>("ModuleFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<ModuleSection> request) =>
        await proxyWrappers.Send<Section?>("ModuleAddSection", request);

    public async Task<Response<Section?>> DuplicateSection(Request<ModuleSection> request) =>
        await proxyWrappers.Send<Section?>("ModuleDuplicateSection", request);

    public async Task<Response> SaveSection(Request<ModuleSection> request) =>
        await proxyWrappers.Send("ModuleSaveSection", request);

    public async Task<Response> RemoveSection(Request<ModuleSection> request) =>
        await proxyWrappers.Send("ModuleRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<ModuleSection> request) =>
        await proxyWrappers.Send("ModuleSaveSectionOrder", request);
}