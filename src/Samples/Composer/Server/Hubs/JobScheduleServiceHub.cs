using Crudspa.Framework.Jobs.Shared.Contracts.Data;
using Crudspa.Framework.Jobs.Shared.Contracts.Events;
using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Composer.Server.Hubs;

public partial class ComposerHub
{
    public async Task<Response<IList<JobSchedule>>> JobScheduleFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobScheduleService.FetchAll(request));
    }

    public async Task<Response<JobSchedule?>> JobScheduleFetch(Request<JobSchedule> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobScheduleService.Fetch(request));
    }

    public async Task<Response<JobSchedule?>> JobScheduleAdd(Request<JobSchedule> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
        {
            var response = await JobScheduleService.Add(request);

            if (response.Ok)
            {
                var jobScheduleAdded = new JobScheduleAdded
                {
                    Id = response.Value.Id,
                };

                await GatewayService.Publish(jobScheduleAdded);
            }

            return response;
        });
    }

    public async Task<Response> JobScheduleSave(Request<JobSchedule> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
        {
            var response = await JobScheduleService.Save(request);

            if (response.Ok)
            {
                var jobScheduleSaved = new JobScheduleSaved
                {
                    Id = request.Value.Id,
                };

                await GatewayService.Publish(jobScheduleSaved);
            }

            return response;
        });
    }

    public async Task<Response> JobScheduleRemove(Request<JobSchedule> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
        {
            var response = await JobScheduleService.Remove(request);

            if (response.Ok)
            {
                var jobScheduleRemoved = new JobScheduleRemoved
                {
                    Id = request.Value.Id,
                };

                await GatewayService.Publish(jobScheduleRemoved);
            }

            return response;
        });
    }

    public async Task<Response<IList<JobTypeFull>>> JobScheduleFetchJobTypes(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobScheduleService.FetchJobTypes(request));
    }

    public async Task<Response<IList<Named>>> JobScheduleFetchDeviceNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Jobs, async session =>
            await JobScheduleService.FetchDeviceNames(request));
    }
}