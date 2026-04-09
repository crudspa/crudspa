namespace Crudspa.Framework.Jobs.Shared.Contracts.Behavior;

public interface IJobScheduleService
{
    Task<Response<IList<JobSchedule>>> FetchAll(Request request);
    Task<Response<JobSchedule?>> Fetch(Request<JobSchedule> request);
    Task<Response<JobSchedule?>> Add(Request<JobSchedule> request);
    Task<Response> Save(Request<JobSchedule> request);
    Task<Response> Remove(Request<JobSchedule> request);
    Task<Response<IList<JobTypeFull>>> FetchJobTypes(Request request);
    Task<Response<IList<Named>>> FetchDeviceNames(Request request);
    Task<Response<IList<JobSchedule>?>> FetchBatch(Request<Device> request);
    Task<Response<Job?>> CreateJob(Request<JobSchedule> request);
    Task<Response> RescheduleOverdue(Request<Device> request);
}