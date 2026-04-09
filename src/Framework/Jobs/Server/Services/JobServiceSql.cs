namespace Crudspa.Framework.Jobs.Server.Services;

public class JobServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IJobService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Job>>> Search(Request<JobSearch> request)
    {
        return await wrappers.Try<IList<Job>>(request, async response =>
        {
            return await JobSelectWhere.Execute(Connection, request.Value);
        });
    }

    public async Task<Response<Job?>> Fetch(Request<Job> request)
    {
        return await wrappers.Try<Job?>(request, async response =>
        {
            var job = await JobSelect.Execute(Connection, request.Value);
            return job;
        });
    }

    public async Task<Response<Job>> Add(Request<Job> request)
    {
        return await wrappers.Validate<Job, Job>(request, async response =>
        {
            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await JobInsert.Execute(connection, transaction, request.SessionId, request.Value);
                return new Job { Id = id };
            });
        });
    }

    public async Task<Response> Remove(Request<Job> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var job = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await JobDelete.Execute(connection, transaction, request.SessionId, job);
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

    public async Task<Response<IList<Named>>> FetchJobTypeNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await JobTypeSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<OrderableCssClass>>> FetchJobStatusNames(Request request)
    {
        return await wrappers.Try<IList<OrderableCssClass>>(request, async response =>
            await JobStatusSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchJobScheduleNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await JobScheduleSelectNames.Execute(Connection));
    }
}