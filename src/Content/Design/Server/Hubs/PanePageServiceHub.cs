using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Orderable>>> PanePageFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PanePageService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Page>>> PanePageFetchPages(Request<PageForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PanePageService.FetchPages(request));
    }

    public async Task<Response<Page?>> PanePageFetchPage(Request<PageForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PanePageService.FetchPage(request));
    }

    public async Task<Response<Page?>> PanePageAddPage(Request<PageForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PanePageService.AddPage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new PageAdded
                {
                    Id = response.Value.Id,
                    BinderId = response.Value.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> PanePageSavePage(Request<PageForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PanePageService.SavePage(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Segments, new PageSaved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Page?.Id });
            }

            return response;
        });
    }

    public async Task<Response> PanePageSavePageOrder(Request<PageForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PanePageService.SavePageOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new PagesReordered
                {
                    BinderId = request.Value.Pages.FirstOrDefault()?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Section>>> PanePageFetchSections(Request<SectionForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PanePageService.FetchSections(request));
    }

    public async Task<Response<Section?>> PanePageFetchSection(Request<SectionForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PanePageService.FetchSection(request));
    }

    public async Task<Response<Section?>> PanePageAddSection(Request<SectionForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PanePageService.AddSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Segments, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = response.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = response.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> PanePageSaveSection(Request<SectionForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PanePageService.SaveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Segments, new SectionSaved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.Section?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Section?.PageId });
            }

            return response;
        });
    }

    public async Task<Response> PanePageRemoveSection(Request<SectionForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PanePageService.RemoveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Segments, new SectionRemoved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.Section?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Section?.PageId });
            }

            return response;
        });
    }

    public async Task<Response> PanePageSaveSectionOrder(Request<SectionForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PanePageService.SaveSectionOrder(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Segments, new SectionsReordered
                {
                    PageId = request.Value.Sections.FirstOrDefault()?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Sections.FirstOrDefault()?.PageId });
            }

            return response;
        });
    }

    public async Task<Response<Binder?>> PanePageAddBinder(Request<BinderForPane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PanePageService.AddBinder(request));
    }
}