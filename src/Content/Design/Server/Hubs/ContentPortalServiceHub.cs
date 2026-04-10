using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<ContentPortal>>> ContentPortalFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.FetchAll(request));
    }

    public async Task<Response<ContentPortal?>> ContentPortalFetch(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.Fetch(request));
    }

    public async Task<Response> ContentPortalSave(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
        {
            var response = await ContentPortalService.Save(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Portals, new ContentPortalSaved
                {
                    Id = request.Value.Id,
                });

                await GatewayService.Publish(new PortalRunChanged { Id = request.Value.Id });
            }

            return response;
        });
    }

    public async Task<Response<IList<Named>>> ContentPortalFetchBlogNames(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.FetchBlogNames(request));
    }

    public async Task<Response<IList<Named>>> ContentPortalFetchCourseNames(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.FetchCourseNames(request));
    }

    public async Task<Response<IList<Named>>> ContentPortalFetchPostNames(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.FetchPostNames(request));
    }

    public async Task<Response<IList<Named>>> ContentPortalFetchTrackNames(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.FetchTrackNames(request));
    }

    public async Task<Response<IList<Section>>> ContentPortalFetchSections(Request<ContentPortalSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.FetchSections(request));
    }

    public async Task<Response<Section?>> ContentPortalFetchSection(Request<ContentPortalSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ContentPortalService.FetchSection(request));
    }

    public async Task<Response<Section?>> ContentPortalAddSection(Request<ContentPortalSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
        {
            var response = await ContentPortalService.AddSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Portals, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = response.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = response.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response<Section?>> ContentPortalDuplicateSection(Request<ContentPortalSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
        {
            var response = await ContentPortalService.DuplicateSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Portals, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = response.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = response.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> ContentPortalSaveSection(Request<ContentPortalSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
        {
            var response = await ContentPortalService.SaveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Portals, new SectionSaved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.Section?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Section?.PageId });
            }

            return response;
        });
    }

    public async Task<Response> ContentPortalRemoveSection(Request<ContentPortalSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
        {
            var response = await ContentPortalService.RemoveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Portals, new SectionRemoved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.Section?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Section?.PageId });
            }

            return response;
        });
    }

    public async Task<Response> ContentPortalSaveSectionOrder(Request<ContentPortalSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
        {
            var response = await ContentPortalService.SaveSectionOrder(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Portals, new SectionsReordered
                {
                    PageId = request.Value.Sections.FirstOrDefault()?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Sections.FirstOrDefault()?.PageId });
            }

            return response;
        });
    }
}