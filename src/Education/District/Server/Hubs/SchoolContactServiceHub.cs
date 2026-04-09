using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.District.Server.Hubs;

public partial class DistrictHub
{
    public async Task<Response<IList<SchoolContact>>> SchoolContactSearchForSchool(Request<SchoolContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SchoolContactService.SearchForSchool(request);
        });
    }

    public async Task<Response<SchoolContact?>> SchoolContactFetch(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolContactService.Fetch(request));
    }

    public async Task<Response<SchoolContact?>> SchoolContactAdd(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            var response = await SchoolContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Schools, new SchoolContactAdded
                {
                    Id = response.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }

    public async Task<Response> SchoolContactSave(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            var response = await SchoolContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Schools, new SchoolContactSaved
                {
                    Id = request.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }

    public async Task<Response> SchoolContactRemove(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            var response = await SchoolContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Schools, new SchoolContactRemoved
                {
                    Id = request.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> SchoolContactFetchTitleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolContactService.FetchTitleNames(request));
    }

    public async Task<Response<IList<Named>>> SchoolContactFetchRoleNames(Request<School> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolContactService.FetchRoleNames(request));
    }

    public async Task<Response> SchoolContactSendAccessCode(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolContactService.SendAccessCode(request));
    }

    public async Task<Response<SchoolContact>> SchoolContactFetchRelations(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolContactService.FetchRelations(request));
    }

    public async Task<Response> SchoolContactSaveRelations(Request<SchoolContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            var response = await SchoolContactService.SaveRelations(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Schools, new SchoolContactRelationsSaved
                {
                    Id = request.Value.Id,
                    SchoolId = request.Value.SchoolId,
                });

            return response;
        });
    }
}