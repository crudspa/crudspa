using SessionInsert = Crudspa.Framework.Jobs.Server.Sproxies.SessionInsert;

namespace Crudspa.Framework.Jobs.Server.Services;

public class JobRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService serverConfigService,
    IJobsConfigService jobsConfigService,
    ICryptographyService cryptographyService)
    : IJobRunService
{
    private String Connection => serverConfigService.Fetch().Database;

    public async Task<Response<Session>> CreateSession(Request request)
    {
        return await wrappers.Try<Session>(request, async response =>
        {
            var sessionId = cryptographyService.GetRandomGuid();
            var serverConfig = serverConfigService.Fetch();
            var jobsConfig = jobsConfigService.Fetch();

            return (await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SessionInsert.Execute(connection, transaction, sessionId, serverConfig.PortalId, jobsConfig.UserId);
                return await SessionSelect.Execute(Connection, sessionId, serverConfig.PortalId);
            }))!;
        });
    }

    public async Task<Response<IList<Job>?>> FetchBatch(Request<Device> request)
    {
        return await wrappers.Try<IList<Job>?>(request, async response =>
            await JobSelectBatch.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<Job?>> FetchSingle(Request<Job> request)
    {
        return await wrappers.Try<Job?>(request, async response =>
        {
            var job = await JobSelectSingle.Execute(Connection, request.SessionId, request.Value.DeviceId, request.Value.Id);
            return job;
        });
    }

    public async Task<Response> SaveStatus(Request<Job> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await JobUpdateStatus.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response> CancelRunning(Request<Device> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var device = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await JobCancelRunning.Execute(connection, transaction, device.Id);
            });
        });
    }
}