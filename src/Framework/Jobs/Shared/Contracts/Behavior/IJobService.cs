namespace Crudspa.Framework.Jobs.Shared.Contracts.Behavior;

public interface IJobService
{
    Task<Response<IList<Job>>> Search(Request<JobSearch> request);
    Task<Response<Job?>> Fetch(Request<Job> request);
    Task<Response<Job>> Add(Request<Job> request);
    Task<Response> Remove(Request<Job> request);
    Task<Response<IList<JobTypeFull>>> FetchJobTypes(Request request);
    Task<Response<IList<Named>>> FetchDeviceNames(Request request);
    Task<Response<IList<Named>>> FetchJobTypeNames(Request request);
    Task<Response<IList<OrderableCssClass>>> FetchJobStatusNames(Request request);
    Task<Response<IList<Named>>> FetchJobScheduleNames(Request request);
}