namespace Crudspa.Education.School.Shared.Contracts.Behavior;

public interface IAssessmentAssignmentService
{
    Task<Response<IList<AssessmentAssignment>>> Search(Request<AssessmentAssignmentSearch> request);
    Task<Response<AssessmentAssignment?>> Fetch(Request<AssessmentAssignment> request);
    Task<Response<Assessment?>> FetchProgress(Request<AssessmentAssignment> request);
    Task<Response<AssessmentAssignment?>> Add(Request<AssessmentAssignment> request);
    Task<Response> Save(Request<AssessmentAssignment> request);
    Task<Response> Remove(Request<AssessmentAssignment> request);
    Task<Response> ResetProgress(Request<AssessmentAssignment> request);
    Task<Response<IList<Named>>> FetchStudentNames(Request request);
    Task<Response<IList<Named>>> FetchAssessmentNames(Request request);
    Task<Response<IList<Named>>> FetchClassroomNames(Request request);
    Task<Response<AssessmentAssignmentBulk>> BulkAssign(Request<AssessmentAssignmentBulk> request);
}