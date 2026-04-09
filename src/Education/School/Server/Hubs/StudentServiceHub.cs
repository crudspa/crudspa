using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<IList<Student>>> StudentSearch(Request<StudentSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
        {
            // Filter by the current school year
            var schoolYearResponse = await SchoolYearService.FetchCurrent(new(session.Id));

            if (!schoolYearResponse.Ok)
            {
                var response = new Response<IList<Student>>();
                response.AddError("The current school year could not be determined.");
                response.AddErrors(schoolYearResponse.Errors);
                return response;
            }

            request.Value.SchoolYears =
            [
                new()
                {
                    Id = schoolYearResponse.Value.Id,
                    Selected = true,
                },
            ];

            return await StudentService.Search(request);
        });
    }

    public async Task<Response<Student?>> StudentFetch(Request<Student> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
            await StudentService.Fetch(request));
    }

    public async Task<Response<Student?>> StudentAdd(Request<Student> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
        {
            var response = await StudentService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Students, new StudentAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> StudentSave(Request<Student> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
        {
            var response = await StudentService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Students, new StudentSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> StudentRemove(Request<Student> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
        {
            var response = await StudentService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Students, new StudentRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> StudentFetchGrades(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
            await StudentService.FetchGrades(request));
    }

    public async Task<Response<IList<Orderable>>> StudentFetchAssessmentTypes(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
            await StudentService.FetchAssessmentTypes(request));
    }

    public async Task<Response<IList<Orderable>>> StudentFetchAssessmentLevels(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
            await StudentService.FetchAssessmentLevels(request));
    }

    public async Task<Response<IList<Named>>> StudentFetchSchoolYears(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
            await StudentService.FetchSchoolYears(request));
    }

    public async Task<Response<Student>> StudentGenerateSecretCode(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
            await StudentService.GenerateSecretCode(request));
    }

    public async Task<Response<District?>> StudentFetchDistrict(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
            await StudentService.FetchDistrict(request));
    }

    public async Task<Response<IList<Selectable>>> StudentFetchSelectableClassrooms(Request<StudentClassroomSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Students, async session =>
        {
            // Filter by the current school year
            var schoolYearResponse = await SchoolYearService.FetchCurrent(new(session.Id));

            if (!schoolYearResponse.Ok)
                return new() { Errors = schoolYearResponse.Errors };

            request.Value.SchoolYearId = schoolYearResponse.Value.Id;

            return await StudentService.FetchSelectableClassrooms(request);
        });
    }
}