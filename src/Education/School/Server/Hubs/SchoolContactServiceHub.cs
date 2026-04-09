using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<IList<SchoolContact>>> SchoolContactSearch(Request<SchoolContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SchoolContactService.Search(request);
        });
    }

    public async Task<Response<SchoolContact?>> SchoolContactFetch(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await SchoolContactService.Fetch(request));
    }

    public async Task<Response<SchoolContact?>> SchoolContactAdd(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await SchoolContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new SchoolContactAdded
                {
                    Id = response.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }

    public async Task<Response> SchoolContactSave(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await SchoolContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new SchoolContactSaved
                {
                    Id = request.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }

    public async Task<Response> SchoolContactRemove(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await SchoolContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new SchoolContactRemoved
                {
                    Id = request.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> SchoolContactFetchTitleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await SchoolContactService.FetchTitleNames(request));
    }

    public async Task<Response<IList<Named>>> SchoolContactFetchRoleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await SchoolContactService.FetchRoleNames(request));
    }

    public async Task<Response> SchoolContactSendAccessCode(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await SchoolContactService.SendAccessCode(request));
    }

    public async Task<Response<SchoolContact>> SchoolContactFetchRelations(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await SchoolContactService.FetchRelations(request));
    }

    public async Task<Response> SchoolContactSaveRelations(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await SchoolContactService.SaveRelations(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new SchoolContactRelationsSaved
                {
                    Id = request.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }
}