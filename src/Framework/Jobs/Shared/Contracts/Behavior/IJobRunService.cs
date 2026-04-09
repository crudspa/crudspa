namespace Crudspa.Framework.Jobs.Shared.Contracts.Behavior;

public interface IJobRunService
{
    Task<Response<Session>> CreateSession(Request request);
    Task<Response<IList<Job>?>> FetchBatch(Request<Device> request);
    Task<Response<Job?>> FetchSingle(Request<Job> request);
    Task<Response> SaveStatus(Request<Job> request);
    Task<Response> CancelRunning(Request<Device> request);
}