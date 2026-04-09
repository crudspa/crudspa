using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Style>>> StyleFetchForContentPortal(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
            await StyleService.FetchForContentPortal(request));
    }

    public async Task<Response<Style?>> StyleFetch(Request<Style> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
            await StyleService.Fetch(request));
    }

    public async Task<Response> StyleSave(Request<Style> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
        {
            var response = await StyleService.Save(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Styles, new StyleSaved
                {
                    Id = request.Value.Id,
                    ContentPortalId = request.Value.ContentPortalId,
                });

                if (request.Value.ContentPortalId.HasValue)
                    await GatewayService.Publish(new PortalRunChanged { Id = request.Value.ContentPortalId.Value });
            }

            return response;
        });
    }

    public async Task<Response<IList<RuleFull>>> StyleFetchRules(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
            await StyleService.FetchRules(request));
    }
}