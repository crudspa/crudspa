using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<IList<ClassRecording>>> ClassRecordingFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.ClassRecording, async session =>
            await ClassRecordingService.FetchAll(request));
    }

    public async Task<Response<ClassRecording?>> ClassRecordingFetch(Request<ClassRecording> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.ClassRecording, async session =>
            await ClassRecordingService.Fetch(request));
    }

    public async Task<Response<ClassRecording?>> ClassRecordingAdd(Request<ClassRecording> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.ClassRecording, async session =>
        {
            var response = await ClassRecordingService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.ClassRecording, new ClassRecordingAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ClassRecordingSave(Request<ClassRecording> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.ClassRecording, async session =>
        {
            var response = await ClassRecordingService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.ClassRecording, new ClassRecordingSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ClassRecordingRemove(Request<ClassRecording> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.ClassRecording, async session =>
        {
            var response = await ClassRecordingService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.ClassRecording, new ClassRecordingRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ClassRecordingFetchContentCategoryNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.ClassRecording, async session =>
            await ClassRecordingService.FetchContentCategoryNames(request));
    }
}