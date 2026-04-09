using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<IList<Report>>> ReportFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Reports, async session =>
            await ReportService.FetchAll(request));
    }

    public async Task<Response<Report?>> ReportFetch(Request<Report> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Reports, async session =>
            await ReportService.Fetch(request));
    }

    public async Task<Response<IList<Named>>> ReportFetchReportNames(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await ReportService.FetchReportNames(request));
    }
}