namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IActivityService
{
    Task<Response<Activity?>> Fetch(Request<Activity> request);
    Task<Response<Activity?>> Add(Request<Activity> request);
    Task<Response> Save(Request<Activity> request);
    Task<Response> Remove(Request<Activity> request);
}