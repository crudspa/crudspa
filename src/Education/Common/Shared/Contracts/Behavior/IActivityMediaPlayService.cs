namespace Crudspa.Education.Common.Shared.Contracts.Behavior;

public interface IActivityMediaPlayService
{
    Task<Response> Add(Request<ActivityMediaPlay> request);
}