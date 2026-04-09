namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IModuleService
{
    Task<Response<IList<Module>>> FetchForBook(Request<Book> request);
    Task<Response<Module?>> Fetch(Request<Module> request);
    Task<Response<Module?>> Add(Request<Module> request);
    Task<Response> Save(Request<Module> request);
    Task<Response> Remove(Request<Module> request);
    Task<Response> SaveOrder(Request<IList<Module>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<IconFull>>> FetchIcons(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request request);
    Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request);
    Task<Response<IList<Page>>> FetchPages(Request<ModulePage> request);
    Task<Response<Page?>> FetchPage(Request<ModulePage> request);
    Task<Response<Page?>> AddPage(Request<ModulePage> request);
    Task<Response> SavePage(Request<ModulePage> request);
    Task<Response> RemovePage(Request<ModulePage> request);
    Task<Response> SavePageOrder(Request<ModulePage> request);
    Task<Response<IList<Section>>> FetchSections(Request<ModuleSection> request);
    Task<Response<Section?>> FetchSection(Request<ModuleSection> request);
    Task<Response<Section?>> AddSection(Request<ModuleSection> request);
    Task<Response> SaveSection(Request<ModuleSection> request);
    Task<Response> RemoveSection(Request<ModuleSection> request);
    Task<Response> SaveSectionOrder(Request<ModuleSection> request);
}