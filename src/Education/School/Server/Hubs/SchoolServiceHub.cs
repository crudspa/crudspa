using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.School.Server.Hubs;

using School = Shared.Contracts.Data.School;

public partial class SchoolHub
{
    public async Task<Response<School?>> SchoolFetch(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await SchoolService.Fetch(request));
    }

    public async Task<Response> SchoolSave(Request<School> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await SchoolService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new SchoolSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> SchoolFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await SchoolService.FetchPermissionNames(request));
    }
}