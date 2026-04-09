using Crudspa.Framework.Jobs.Shared.Extensions;

namespace Crudspa.Framework.Jobs.Server.Services;

public class JobScheduleServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IJobScheduleService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<JobSchedule>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<JobSchedule>>(request, async response =>
        {
            var jobSchedules = await JobScheduleSelectAll.Execute(Connection);
            return jobSchedules;
        });
    }

    public async Task<Response<JobSchedule?>> Fetch(Request<JobSchedule> request)
    {
        return await wrappers.Try<JobSchedule?>(request, async response =>
        {
            var jobSchedule = await JobScheduleSelect.Execute(Connection, request.Value);
            return jobSchedule;
        });
    }

    public async Task<Response<JobSchedule?>> Add(Request<JobSchedule> request)
    {
        return await wrappers.Validate<JobSchedule?, JobSchedule>(request, async response =>
        {
            var jobSchedule = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await JobScheduleInsert.Execute(connection, transaction, request.SessionId, jobSchedule);

                return new JobSchedule
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<JobSchedule> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var jobSchedule = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await JobScheduleUpdate.Execute(connection, transaction, request.SessionId, jobSchedule);
            });
        });
    }

    public async Task<Response> Remove(Request<JobSchedule> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var jobSchedule = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await JobScheduleDelete.Execute(connection, transaction, request.SessionId, jobSchedule);
            });
        });
    }

    public async Task<Response<IList<JobTypeFull>>> FetchJobTypes(Request request)
    {
        return await wrappers.Try<IList<JobTypeFull>>(request, async response =>
            await JobTypeSelectFull.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchDeviceNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await DeviceSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<JobSchedule>?>> FetchBatch(Request<Device> request)
    {
        return await wrappers.Try<IList<JobSchedule>?>(request, async response =>
            await JobScheduleSelectBatch.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<Job?>> CreateJob(Request<JobSchedule> request)
    {
        return await wrappers.Try<Job?>(request, async response =>
        {
            var jobSchedule = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var jobId = await JobScheduleCreateJob.Execute(connection, transaction, request.SessionId, jobSchedule);

                return jobId.HasValue
                    ? new Job { Id = jobId, ScheduleId = jobSchedule.Id }
                    : null;
            });
        });
    }

    public async Task<Response> RescheduleOverdue(Request<Device> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var device = request.Value;

            var overdue = await JobScheduleSelectOverdue.Execute(Connection, request.SessionId, device.Id) ?? [];

            foreach (var schedule in overdue)
            {
                schedule.NextRun = schedule.DetermineNextRunDate();

                await sqlWrappers.WithConnection(async (connection, transaction) =>
                {
                    await JobScheduleUpdateNextRun.Execute(connection, transaction, request.SessionId, schedule);
                });
            }
        });
    }
}