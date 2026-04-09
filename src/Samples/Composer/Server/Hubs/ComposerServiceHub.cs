using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Composer.Server.Hubs;

using Composer = Shared.Contracts.Data.Composer;

public partial class ComposerHub
{
    public async Task<Response<Composer?>> ComposerFetch(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await ComposerService.Fetch(request));
    }

    public async Task<Response> ComposerSave(Request<Composer> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await ComposerService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new ComposerSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> ComposerFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await ComposerService.FetchPermissionNames(request));
    }
}