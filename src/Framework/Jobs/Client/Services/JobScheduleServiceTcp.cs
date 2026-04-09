namespace Crudspa.Framework.Jobs.Client.Services;

public class JobScheduleServiceTcp(IProxyWrappers proxyWrappers) : IJobScheduleService
{
    public async Task<Response<IList<JobSchedule>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<JobSchedule>>("JobScheduleFetchAll", request);

    public async Task<Response<JobSchedule?>> Fetch(Request<JobSchedule> request) =>
        await proxyWrappers.Send<JobSchedule?>("JobScheduleFetch", request);

    public async Task<Response<JobSchedule?>> Add(Request<JobSchedule> request) =>
        await proxyWrappers.Send<JobSchedule?>("JobScheduleAdd", request);

    public async Task<Response> Save(Request<JobSchedule> request) =>
        await proxyWrappers.Send("JobScheduleSave", request);

    public async Task<Response> Remove(Request<JobSchedule> request) =>
        await proxyWrappers.Send("JobScheduleRemove", request);

    public async Task<Response<IList<JobTypeFull>>> FetchJobTypes(Request request) =>
        await proxyWrappers.SendAndCache<IList<JobTypeFull>>("JobScheduleFetchJobTypes", request);

    public async Task<Response<IList<Named>>> FetchDeviceNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Named>>("JobScheduleFetchDeviceNames", request);

    public Task<Response<IList<JobSchedule>?>> FetchBatch(Request<Device> request) =>
        throw new NotImplementedException("Implemented server side only.");

    public Task<Response<Job?>> CreateJob(Request<JobSchedule> request) =>
        throw new NotImplementedException("Implemented server side only.");

    public Task<Response> RescheduleOverdue(Request<Device> request) =>
        throw new NotImplementedException("Implemented server side only.");
}