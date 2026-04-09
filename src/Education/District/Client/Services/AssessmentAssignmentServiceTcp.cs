namespace Crudspa.Education.District.Client.Services;

public class AssessmentAssignmentServiceTcp(IProxyWrappers proxyWrappers) : IAssessmentAssignmentService
{
    public async Task<Response<IList<AssessmentAssignment>>> Search(Request<AssessmentAssignmentSearch> request) =>
        await proxyWrappers.Send<IList<AssessmentAssignment>>("AssessmentAssignmentSearch", request);

    public async Task<Response<AssessmentAssignment?>> Fetch(Request<AssessmentAssignment> request) =>
        await proxyWrappers.Send<AssessmentAssignment?>("AssessmentAssignmentFetch", request);

    public async Task<Response<Assessment?>> FetchProgress(Request<AssessmentAssignment> request) =>
        await proxyWrappers.Send<Assessment?>("AssessmentAssignmentFetchProgress", request);

    public async Task<Response<AssessmentAssignment?>> Add(Request<AssessmentAssignment> request) =>
        await proxyWrappers.Send<AssessmentAssignment?>("AssessmentAssignmentAdd", request);

    public async Task<Response> Save(Request<AssessmentAssignment> request) =>
        await proxyWrappers.Send("AssessmentAssignmentSave", request);

    public async Task<Response> Remove(Request<AssessmentAssignment> request) =>
        await proxyWrappers.Send("AssessmentAssignmentRemove", request);

    public async Task<Response<IList<Named>>> FetchStudentNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("AssessmentAssignmentFetchStudentNames", request);

    public async Task<Response<IList<Named>>> FetchAssessmentNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("AssessmentAssignmentFetchAssessmentNames", request);

    public async Task<Response<IList<Named>>> FetchClassroomNames(Request<School> request) =>
        await proxyWrappers.Send<IList<Named>>("AssessmentAssignmentFetchClassroomNames", request);

    public async Task<Response<IList<Named>>> FetchCommunityNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("AssessmentAssignmentFetchCommunityNames", request);

    public async Task<Response<IList<Named>>> FetchSchoolNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("AssessmentAssignmentFetchSchoolNames", request);

    public async Task<Response<AssessmentAssignmentBulk>> BulkAssign(Request<AssessmentAssignmentBulk> request) =>
        await proxyWrappers.Send<AssessmentAssignmentBulk>("AssessmentAssignmentBulkAssign", request);
}