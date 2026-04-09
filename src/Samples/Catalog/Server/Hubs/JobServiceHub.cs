using Crudspa.Framework.Jobs.Shared.Contracts.Data;
using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub
{
    public async Task<Response<IList<Job>>> JobSearch(Request<JobSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await JobService.Search(request);
        });
    }

    public async Task<Response<Job?>> JobFetch(Request<Job> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobService.Fetch(request));
    }

    public async Task<Response> JobAdd(Request<Job> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
        {
            var response = await JobService.Add(request);

            if (response.Ok)
            {
                var jobAdded = new JobAdded { Id = response.Value.Id };
                await GatewayService.Publish(jobAdded);
            }

            return response;
        });
    }

    public async Task<Response> JobRemove(Request<Job> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
        {
            var response = await JobService.Remove(request);

            if (response.Ok)
            {
                var jobRemoved = new JobRemoved { Id = request.Value.Id };
                await GatewayService.Publish(jobRemoved);
            }

            return response;
        });
    }

    public async Task<Response> JobNotifySaved(Request<JobPayload> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
        {
            if (request.Value.Id is null)
                return new("JobNotifySaved requires a job id.");

            var jobSaved = new JobSaved
            {
                Id = request.Value.Id,
            };

            await GatewayService.Publish(jobSaved);

            return new();
        });
    }

    public async Task<Response<IList<JobTypeFull>>> JobFetchJobTypes(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobService.FetchJobTypes(request));
    }

    public async Task<Response<IList<Named>>> JobFetchDeviceNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobService.FetchDeviceNames(request));
    }

    public async Task<Response<IList<Named>>> JobFetchJobTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobService.FetchJobTypeNames(request));
    }

    public async Task<Response<IList<OrderableCssClass>>> JobFetchJobStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobService.FetchJobStatusNames(request));
    }

    public async Task<Response<IList<Named>>> JobFetchJobScheduleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobService.FetchJobScheduleNames(request));
    }
}