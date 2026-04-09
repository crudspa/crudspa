namespace Crudspa.Framework.Jobs.Client.Services;

public class JobServiceTcp(IProxyWrappers proxyWrappers) : IJobService
{
    public async Task<Response<IList<Job>>> Search(Request<JobSearch> request) =>
        await proxyWrappers.Send<IList<Job>>("JobSearch", request);

    public async Task<Response<Job?>> Fetch(Request<Job> request) =>
        await proxyWrappers.Send<Job?>("JobFetch", request);

    public async Task<Response<Job>> Add(Request<Job> request) =>
        await proxyWrappers.Send<Job>("JobAdd", request);

    public async Task<Response> Remove(Request<Job> request) =>
        await proxyWrappers.Send("JobRemove", request);

    public async Task<Response<IList<JobTypeFull>>> FetchJobTypes(Request request) =>
        await proxyWrappers.SendAndCache<IList<JobTypeFull>>("JobFetchJobTypes", request);

    public async Task<Response<IList<Named>>> FetchDeviceNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Named>>("JobFetchDeviceNames", request);

    public async Task<Response<IList<Named>>> FetchJobTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Named>>("JobFetchJobTypeNames", request);

    public async Task<Response<IList<OrderableCssClass>>> FetchJobStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<OrderableCssClass>>("JobFetchJobStatusNames", request);

    public async Task<Response<IList<Named>>> FetchJobScheduleNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("JobFetchJobScheduleNames", request);
}