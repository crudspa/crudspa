using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<IList<AssessmentAssignment>>> AssessmentAssignmentSearch(Request<AssessmentAssignmentSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentAssignmentService.Search(request));
    }

    public async Task<Response<AssessmentAssignment?>> AssessmentAssignmentFetch(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentAssignmentService.Fetch(request));
    }

    public async Task<Response<Assessment?>> AssessmentAssignmentFetchProgress(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentAssignmentService.FetchProgress(request));
    }

    public async Task<Response<AssessmentAssignment?>> AssessmentAssignmentAdd(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await AssessmentAssignmentService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new AssessmentAssignmentAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> AssessmentAssignmentSave(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await AssessmentAssignmentService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new AssessmentAssignmentSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> AssessmentAssignmentRemove(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await AssessmentAssignmentService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new AssessmentAssignmentRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> AssessmentAssignmentResetProgress(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await AssessmentAssignmentService.ResetProgress(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new AssessmentAssignmentSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> AssessmentAssignmentFetchAssessmentNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentAssignmentService.FetchAssessmentNames(request));
    }

    public async Task<Response<IList<Named>>> AssessmentAssignmentFetchStudentNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentAssignmentService.FetchStudentNames(request));
    }

    public async Task<Response<IList<Named>>> AssessmentAssignmentFetchClassroomNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentAssignmentService.FetchClassroomNames(request));
    }

    public async Task<Response<AssessmentAssignmentBulk>> AssessmentAssignmentBulkAssign(Request<AssessmentAssignmentBulk> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentAssignmentService.BulkAssign(request));
    }
}