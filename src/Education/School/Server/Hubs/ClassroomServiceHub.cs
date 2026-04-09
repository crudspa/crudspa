using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<IList<Classroom>>> ClassroomFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
            await ClassroomService.FetchAll(request));
    }

    public async Task<Response<Classroom?>> ClassroomFetch(Request<Classroom> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
            await ClassroomService.Fetch(request));
    }

    public async Task<Response<Classroom?>> ClassroomAdd(Request<Classroom> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
        {
            var response = await ClassroomService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Classrooms, new ClassroomAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ClassroomSave(Request<Classroom> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
        {
            var response = await ClassroomService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Classrooms, new ClassroomSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ClassroomRemove(Request<Classroom> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
        {
            var response = await ClassroomService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Classrooms, new ClassroomRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Selectable>>> ClassroomFetchStudents(Request<Classroom> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
            await ClassroomService.FetchStudents(request));
    }

    public async Task<Response<IList<Selectable>>> ClassroomFetchSchoolContacts(Request<Classroom> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
            await ClassroomService.FetchSchoolContacts(request));
    }

    public async Task<Response<IList<Orderable>>> ClassroomFetchClassroomTypes(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
            await ClassroomService.FetchTypeNames(request));
    }

    public async Task<Response<IList<Orderable>>> ClassroomFetchClassroomGrades(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Classrooms, async session =>
            await ClassroomService.FetchGradeNames(request));
    }

    public async Task<Response<IList<Named>>> ClassroomFetchClassroomNames(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ClassroomService.FetchClassroomNames(request));
    }

    public async Task<Response<District?>> ClassroomFetchDistrict(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ClassroomService.FetchDistrict(request));
    }
}