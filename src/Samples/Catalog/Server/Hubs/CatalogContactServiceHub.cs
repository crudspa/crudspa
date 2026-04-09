using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub
{
    public async Task<Response<IList<CatalogContact>>> CatalogContactSearch(Request<CatalogContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await CatalogContactService.Search(request);
        });
    }

    public async Task<Response<CatalogContact?>> CatalogContactFetch(Request<CatalogContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await CatalogContactService.Fetch(request));
    }

    public async Task<Response<CatalogContact?>> CatalogContactAdd(Request<CatalogContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await CatalogContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new CatalogContactAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> CatalogContactSave(Request<CatalogContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await CatalogContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new CatalogContactSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> CatalogContactRemove(Request<CatalogContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await CatalogContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new CatalogContactRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> CatalogContactFetchRoleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await CatalogContactService.FetchRoleNames(request));
    }

    public async Task<Response> CatalogContactSendAccessCode(Request<CatalogContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await CatalogContactService.SendAccessCode(request));
    }
}