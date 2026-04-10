using ContentPortalSelect = Crudspa.Content.Design.Server.Sproxies.ContentPortalSelect;

namespace Crudspa.Content.Design.Server.Services;

public class ContentPortalServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IPagePartsService pagePartsService)
    : IContentPortalService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ContentPortal>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<ContentPortal>>(request, async response =>
        {
            var contentPortals = await ContentPortalSelectAll.Execute(Connection, request.SessionId);
            return contentPortals;
        });
    }

    public async Task<Response<ContentPortal?>> Fetch(Request<ContentPortal> request)
    {
        return await wrappers.Try<ContentPortal?>(request, async response =>
        {
            var contentPortal = await ContentPortalSelect.Execute(Connection, request.SessionId, request.Value);
            return contentPortal;
        });
    }

    public async Task<Response> Save(Request<ContentPortal> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var contentPortal = request.Value;

            var existing = await ContentPortalSelect.Execute(Connection, request.SessionId, contentPortal);

            var brandingImageFileResponse = await fileService.SaveImage(new(request.SessionId, contentPortal.BrandingImageFile), existing?.BrandingImageFile);
            if (!brandingImageFileResponse.Ok)
            {
                response.AddErrors(brandingImageFileResponse.Errors);
                return;
            }

            contentPortal.BrandingImageFile.Id = brandingImageFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ContentPortalUpdate.Execute(connection, transaction, request.SessionId, contentPortal);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchBlogNames(Request<ContentPortal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await BlogSelectNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Named>>> FetchCourseNames(Request<ContentPortal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await CourseSelectNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Named>>> FetchPostNames(Request<ContentPortal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PostSelectNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Named>>> FetchTrackNames(Request<ContentPortal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await TrackSelectNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<ContentPortalSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var pageId = await FetchFooterPageId(request.SessionId, request.Value.ContentPortalId);

            if (pageId.HasNothing())
                throw new("Footer page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<ContentPortalSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchFooterPageId(request.SessionId, request.Value.ContentPortalId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Footer section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<ContentPortalSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchFooterPageId(request.SessionId, request.Value.ContentPortalId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Footer page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response<Section?>> DuplicateSection(Request<ContentPortalSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchFooterPageId(request.SessionId, request.Value.ContentPortalId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Footer section not found.");

            var duplicateResponse = await pagePartsService.DuplicateSection(request.SessionId, pageId, section);
            response.AddErrors(duplicateResponse.Errors);
            return duplicateResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<ContentPortalSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchFooterPageId(request.SessionId, request.Value.ContentPortalId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Footer section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<ContentPortalSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchFooterPageId(request.SessionId, request.Value.ContentPortalId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Footer section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<ContentPortalSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchFooterPageId(request.SessionId, request.Value.ContentPortalId);
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing())
                throw new("Footer page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchFooterPageId(Guid? sessionId, Guid? contentPortalId)
    {
        var contentPortal = await ContentPortalSelect.Execute(Connection, sessionId, new() { Id = contentPortalId });
        return contentPortal?.FooterPageId;
    }
}