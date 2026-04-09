namespace Crudspa.Content.Design.Client.Services;

public class ContentPortalServiceTcp(IProxyWrappers proxyWrappers) : IContentPortalService
{
    public async Task<Response<IList<ContentPortal>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<ContentPortal>>("ContentPortalFetchAll", request);

    public async Task<Response<ContentPortal?>> Fetch(Request<ContentPortal> request) =>
        await proxyWrappers.Send<ContentPortal?>("ContentPortalFetch", request);

    public async Task<Response> Save(Request<ContentPortal> request) =>
        await proxyWrappers.Send("ContentPortalSave", request);

    public async Task<Response<IList<Named>>> FetchBlogNames(Request<ContentPortal> request) =>
        await proxyWrappers.Send<IList<Named>>("ContentPortalFetchBlogNames", request);

    public async Task<Response<IList<Named>>> FetchCourseNames(Request<ContentPortal> request) =>
        await proxyWrappers.Send<IList<Named>>("ContentPortalFetchCourseNames", request);

    public async Task<Response<IList<Named>>> FetchPostNames(Request<ContentPortal> request) =>
        await proxyWrappers.Send<IList<Named>>("ContentPortalFetchPostNames", request);

    public async Task<Response<IList<Named>>> FetchTrackNames(Request<ContentPortal> request) =>
        await proxyWrappers.Send<IList<Named>>("ContentPortalFetchTrackNames", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<ContentPortalSection> request) =>
        await proxyWrappers.Send<IList<Section>>("ContentPortalFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<ContentPortalSection> request) =>
        await proxyWrappers.Send<Section?>("ContentPortalFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<ContentPortalSection> request) =>
        await proxyWrappers.Send<Section?>("ContentPortalAddSection", request);

    public async Task<Response> SaveSection(Request<ContentPortalSection> request) =>
        await proxyWrappers.Send("ContentPortalSaveSection", request);

    public async Task<Response> RemoveSection(Request<ContentPortalSection> request) =>
        await proxyWrappers.Send("ContentPortalRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<ContentPortalSection> request) =>
        await proxyWrappers.Send("ContentPortalSaveSectionOrder", request);
}