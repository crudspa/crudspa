using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<FontFace>>> FontFaceFetchForFont(Request<Font> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
            await FontFaceService.FetchForFont(request));
    }

    public async Task<Response<FontFace?>> FontFaceFetch(Request<FontFace> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
            await FontFaceService.Fetch(request));
    }

    public async Task<Response<FontFace?>> FontFaceAdd(Request<FontFace> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
        {
            var contentPortalId = await FetchContentPortalIdForFontFace(request);
            var response = await FontFaceService.Add(request);

            if (response.Ok)
                await NotifyFontFaceChanged(request.SessionId, contentPortalId, new FontFaceAdded
                {
                    Id = response.Value.Id,
                    FontId = request.Value.FontId,
                    ContentPortalId = contentPortalId,
                });

            return response;
        });
    }

    public async Task<Response> FontFaceSave(Request<FontFace> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
        {
            var contentPortalId = await FetchContentPortalIdForFontFace(request);
            var response = await FontFaceService.Save(request);

            if (response.Ok)
                await NotifyFontFaceChanged(request.SessionId, contentPortalId, new FontFaceSaved
                {
                    Id = request.Value.Id,
                    FontId = request.Value.FontId,
                    ContentPortalId = contentPortalId,
                });

            return response;
        });
    }

    public async Task<Response> FontFaceRemove(Request<FontFace> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
        {
            var existing = await FontFaceService.Fetch(request);
            var contentPortalId = existing.Ok && existing.Value is not null
                ? await FetchContentPortalIdForFontFace(new(request.SessionId, existing.Value))
                : await FetchContentPortalIdForFontFace(request);

            var response = await FontFaceService.Remove(request);

            if (response.Ok)
                await NotifyFontFaceChanged(request.SessionId, contentPortalId, new FontFaceRemoved
                {
                    Id = request.Value.Id,
                    FontId = existing.Ok && existing.Value is not null ? existing.Value.FontId : request.Value.FontId,
                    ContentPortalId = contentPortalId,
                });

            return response;
        });
    }

    private async Task<Guid?> FetchContentPortalIdForFontFace(Request<FontFace> request)
    {
        var fontId = request.Value.FontId;

        if (!fontId.HasValue && request.Value.Id.HasValue)
        {
            var faceResponse = await FontFaceService.Fetch(request);
            fontId = faceResponse.Ok && faceResponse.Value is not null ? faceResponse.Value.FontId : null;
        }

        if (!fontId.HasValue)
            return null;

        var fontResponse = await FontService.Fetch(new(request.SessionId, new Font { Id = fontId }));
        return fontResponse.Ok ? fontResponse.Value.ContentPortalId : null;
    }

    private async Task NotifyFontFaceChanged<T>(Guid? sessionId, Guid? contentPortalId, T payload)
        where T : FontFacePayload
    {
        await Notify(sessionId, PermissionIds.Styles, payload);

        if (contentPortalId.HasValue)
            await GatewayService.Publish(new PortalRunChanged { Id = contentPortalId.Value });
    }
}