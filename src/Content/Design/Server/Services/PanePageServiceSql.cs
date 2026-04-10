using Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

namespace Crudspa.Content.Design.Server.Services;

public class PanePageServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    IPagePartsService pagePartsService)
    : IPanePageService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Page>>> FetchPages(Request<PageForPane> request)
    {
        return await wrappers.Try<IList<Page>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value);

            if (binderId.HasNothing())
                throw new("Pane binder not found.");

            var fetchResponse = await pagePartsService.FetchPages(request.SessionId, binderId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<PageForPane> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);

            if (pageId.HasNothing())
                throw new("Pane page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, new() { Id = pageId });
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> AddPage(Request<PageForPane> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var page = request.Value.Page;

            if (page is null)
                throw new("Pane page is required.");

            var binderId = await FetchBinderId(request.SessionId, request.Value);

            if (binderId.HasValue)
            {
                var addResponse = await pagePartsService.AddPage(request.SessionId, binderId, page);
                response.AddErrors(addResponse.Errors);
                return addResponse.Value!;
            }

            var (pageId, existingBinderId) = await FetchContentIds(request.SessionId, request.Value.PaneId);

            if (pageId.HasValue || existingBinderId.HasValue)
            {
                response.AddError("Pane content already exists.");
                return null!;
            }

            if (request.Value.PaneId.HasNothing())
                throw new("Pane page not found.");

            var pageResponse = await pagePartsService.AddPage(request.SessionId, page);
            response.AddErrors(pageResponse.Errors);
            return pageResponse.Value;
        });
    }

    public async Task<Response> SavePage(Request<PageForPane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);
            var page = request.Value.Page;

            if (pageId.HasNothing() || page is null)
                throw new("Pane page not found.");

            var existingResponse = await pagePartsService.FetchPage(request.SessionId, new() { Id = pageId });

            if (!existingResponse.Ok)
            {
                response.AddErrors(existingResponse.Errors);
                return;
            }

            page.Id = existingResponse.Value?.Id;
            page.BinderId = existingResponse.Value?.BinderId;

            var saveResponse = await pagePartsService.SavePage(request.SessionId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemovePage(Request<PageForPane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value);
            var page = request.Value.Page;

            if (binderId.HasNothing() || page is null)
                throw new("Pane page not found.");

            var removeResponse = await pagePartsService.RemovePage(request.SessionId, binderId, page);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SavePageOrder(Request<PageForPane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value);
            var pages = request.Value.Pages;

            if (binderId.HasNothing() || !pages.HasItems())
                throw new("Pane binder not found.");

            var saveResponse = await pagePartsService.SavePageOrder(request.SessionId, binderId, pages);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<SectionForPane> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);

            if (pageId.HasNothing())
                throw new("Pane page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<SectionForPane> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);
            var section = request.Value.Section;

            if (pageId.HasNothing() || section is null)
                throw new("Pane section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<SectionForPane> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);
            var section = request.Value.Section;

            if (pageId.HasNothing() || section is null)
                throw new("Pane page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response<Section?>> DuplicateSection(Request<SectionForPane> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);
            var section = request.Value.Section;

            if (pageId.HasNothing() || section is null)
                throw new("Pane section not found.");

            var duplicateResponse = await pagePartsService.DuplicateSection(request.SessionId, pageId, section);
            response.AddErrors(duplicateResponse.Errors);
            return duplicateResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<SectionForPane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);
            var section = request.Value.Section;

            if (pageId.HasNothing() || section is null)
                throw new("Pane section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<SectionForPane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);
            var section = request.Value.Section;

            if (pageId.HasNothing() || section is null)
                throw new("Pane section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<SectionForPane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value);
            var sections = request.Value.Sections;

            if (pageId.HasNothing() || !sections.HasItems())
                throw new("Pane page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<Binder?>> AddBinder(Request<BinderForPane> request)
    {
        return await wrappers.Try<Binder?>(request, async response =>
        {
            if (request.Value.PaneId.HasNothing())
                throw new("Pane not found.");

            var (pageId, binderId) = await FetchContentIds(request.SessionId, request.Value.PaneId);

            if (pageId.HasValue || binderId.HasValue)
            {
                response.AddError("Pane content already exists.");
                return null!;
            }

            var binder = request.Value.Binder;

            if (binder is null)
                throw new("Pane binder is required.");

            var addResponse = await pagePartsService.AddBinder(request.SessionId, binder);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    private async Task<Guid?> FetchBinderId(Guid? sessionId, PageForPane request)
    {
        if (request.BinderId.HasValue && await HasBinder(sessionId, request.BinderId))
            return request.BinderId;

        if (request.Page?.BinderId.HasValue == true && await HasBinder(sessionId, request.Page.BinderId))
            return request.Page.BinderId;

        if (request.Pages.HasItems())
        {
            var binderId = request.Pages.FirstOrDefault()?.BinderId;

            if (binderId.HasValue && await HasBinder(sessionId, binderId))
                return binderId;
        }

        var (_, paneBinderId) = await FetchContentIds(sessionId, request.PaneId);
        return paneBinderId;
    }

    private async Task<Guid?> FetchPageId(Guid? sessionId, PageForPane request)
    {
        if (request.Page?.Id.HasValue == true && await HasPage(sessionId, request.Page.Id))
            return request.Page.Id;

        var (panePageId, _) = await FetchContentIds(sessionId, request.PaneId);

        if (panePageId.HasValue && await HasPage(sessionId, panePageId))
            return panePageId;

        return null;
    }

    private async Task<Guid?> FetchPageId(Guid? sessionId, SectionForPane request)
    {
        var pageId = request.PageId
            ?? request.Section?.PageId
            ?? request.Sections.FirstOrDefault()?.PageId;

        if (pageId.HasValue && await HasPage(sessionId, pageId))
            return pageId;

        var (panePageId, _) = await FetchContentIds(sessionId, request.PaneId);

        if (panePageId.HasValue && await HasPage(sessionId, panePageId))
            return panePageId;

        var segmentPageId = await FetchPageIdForSegment(sessionId, request.SegmentId);

        if (segmentPageId.HasValue && await HasPage(sessionId, segmentPageId))
            return segmentPageId;

        var sectionId = request.Section?.Id
            ?? request.Sections.FirstOrDefault()?.Id;

        var sectionPageId = await FetchPageIdForSection(sessionId, sectionId);

        return sectionPageId.HasValue && await HasPage(sessionId, sectionPageId)
            ? sectionPageId
            : null;
    }

    private async Task<Boolean> HasBinder(Guid? sessionId, Guid? binderId) =>
        binderId.HasValue && await PaneSelectForBinder.Execute(Connection, sessionId, binderId) is not null;

    private async Task<Boolean> HasPage(Guid? sessionId, Guid? pageId) =>
        pageId.HasValue && await PaneSelectForPage.Execute(Connection, sessionId, pageId) is not null;

    private async Task<Guid?> FetchPageIdForSection(Guid? sessionId, Guid? sectionId)
    {
        if (sectionId.HasNothing())
            return null;

        return await SectionSelectPageId.Execute(Connection, sessionId, sectionId);
    }

    private async Task<Guid?> FetchPageIdForSegment(Guid? sessionId, Guid? segmentId)
    {
        if (segmentId.HasNothing())
            return null;

        return await SegmentSelectPageId.Execute(Connection, sessionId, segmentId);
    }

    private async Task<(Guid? PageId, Guid? BinderId)> FetchContentIds(Guid? sessionId, Guid? paneId)
    {
        if (paneId.HasNothing())
            return (null, null);

        var pane = await PaneSelect.Execute(Connection, sessionId, new() { Id = paneId });

        var pageId = pane?.ConfigJson.FromJson<PageConfig>()?.PageId;
        var binderId = pane?.ConfigJson.FromJson<BinderConfig>()?.BinderId;

        return (pageId, binderId);
    }
}