using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Composer.Server.Hubs;

public partial class ComposerHub
{
    public async Task<Response<IList<ComposerContact>>> ComposerContactSearch(Request<ComposerContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await ComposerContactService.Search(request);
        });
    }

    public async Task<Response<ComposerContact?>> ComposerContactFetch(Request<ComposerContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await ComposerContactService.Fetch(request));
    }

    public async Task<Response<ComposerContact?>> ComposerContactAdd(Request<ComposerContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await ComposerContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new ComposerContactAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ComposerContactSave(Request<ComposerContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await ComposerContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new ComposerContactSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ComposerContactRemove(Request<ComposerContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await ComposerContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new ComposerContactRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> ComposerContactFetchRoleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await ComposerContactService.FetchRoleNames(request));
    }

    public async Task<Response> ComposerContactSendAccessCode(Request<ComposerContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await ComposerContactService.SendAccessCode(request));
    }
}