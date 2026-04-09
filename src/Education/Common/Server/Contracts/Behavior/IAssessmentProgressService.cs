namespace Crudspa.Education.Common.Server.Contracts.Behavior;

public interface IAssessmentProgressService
{
    Task<Response<Assessment?>> Fetch(Request<AssessmentAssignment> request);
}