namespace Crudspa.Content.Design.Server.Contracts.Behavior;

public interface IPagePartsService
{
    Task<Response<IList<Page>>> FetchPages(Guid? sessionId, Guid? binderId);
    Task<Response<Page?>> FetchPage(Guid? sessionId, Page page);
    Task<Response<Page?>> FetchPage(Guid? sessionId, Guid? binderId, Page page);
    Task<Response<Page?>> AddPage(Guid? sessionId, Page page);
    Task<Response<Page?>> AddPage(Guid? sessionId, Guid? binderId, Page page);
    Task<Response<Copy>> CopyPage(Guid? sessionId, Copy copy);
    Task<Response> SavePage(Guid? sessionId, Page page);
    Task<Response> SavePage(Guid? sessionId, Guid? binderId, Page page);
    Task<Response> RemovePage(Guid? sessionId, Page page);
    Task<Response> RemovePage(Guid? sessionId, Guid? binderId, Page page);
    Task<Response> SavePageOrder(Guid? sessionId, Guid? binderId, IList<Page> pages);
    Task<Response<Binder?>> AddBinder(Guid? sessionId, Binder binder);
    Task<Response<IList<Section>>> FetchSections(Guid? sessionId, Guid? pageId);
    Task<Response<IList<Section>>> FetchSections(Guid? sessionId, Guid? binderId, Guid? pageId);
    Task<Response<Section?>> FetchSection(Guid? sessionId, Guid? pageId, Section section);
    Task<Response<Section?>> FetchSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section);
    Task<Response<Section?>> AddSection(Guid? sessionId, Guid? pageId, Section section);
    Task<Response<Section?>> AddSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section);
    Task<Response> SaveSection(Guid? sessionId, Guid? pageId, Section section);
    Task<Response> SaveSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section);
    Task<Response> RemoveSection(Guid? sessionId, Guid? pageId, Section section);
    Task<Response> RemoveSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section);
    Task<Response> SaveSectionOrder(Guid? sessionId, Guid? pageId, IList<Section> sections);
    Task<Response> SaveSectionOrder(Guid? sessionId, Guid? binderId, Guid? pageId, IList<Section> sections);
}