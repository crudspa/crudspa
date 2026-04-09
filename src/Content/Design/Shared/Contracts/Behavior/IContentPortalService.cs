namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IContentPortalService
{
    Task<Response<IList<ContentPortal>>> FetchAll(Request request);
    Task<Response<ContentPortal?>> Fetch(Request<ContentPortal> request);
    Task<Response> Save(Request<ContentPortal> request);
    Task<Response<IList<Named>>> FetchBlogNames(Request<ContentPortal> request);
    Task<Response<IList<Named>>> FetchCourseNames(Request<ContentPortal> request);
    Task<Response<IList<Named>>> FetchPostNames(Request<ContentPortal> request);
    Task<Response<IList<Named>>> FetchTrackNames(Request<ContentPortal> request);
    Task<Response<IList<Section>>> FetchSections(Request<ContentPortalSection> request);
    Task<Response<Section?>> FetchSection(Request<ContentPortalSection> request);
    Task<Response<Section?>> AddSection(Request<ContentPortalSection> request);
    Task<Response> SaveSection(Request<ContentPortalSection> request);
    Task<Response> RemoveSection(Request<ContentPortalSection> request);
    Task<Response> SaveSectionOrder(Request<ContentPortalSection> request);
}