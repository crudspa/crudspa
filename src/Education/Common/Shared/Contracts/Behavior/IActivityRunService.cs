namespace Crudspa.Education.Common.Shared.Contracts.Behavior;

public interface IActivityRunService
{
    Task<Response<ActivityAssignment>> AddActivityState(Request<ActivityAssignment> request);
    Task<Response> SaveActivityState(Request<ActivityAssignment> request);
}