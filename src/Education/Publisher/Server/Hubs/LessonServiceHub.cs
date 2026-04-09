using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Lesson>>> LessonFetchForUnit(Request<Unit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await LessonService.FetchForUnit(request));
    }

    public async Task<Response<Lesson?>> LessonFetch(Request<Lesson> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await LessonService.Fetch(request));
    }

    public async Task<Response<Lesson?>> LessonAdd(Request<Lesson> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await LessonService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new LessonAdded
                {
                    Id = response.Value.Id,
                    UnitId = request.Value.UnitId,
                });

            return response;
        });
    }

    public async Task<Response> LessonSave(Request<Lesson> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await LessonService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new LessonSaved
                {
                    Id = request.Value.Id,
                    UnitId = request.Value.UnitId,
                });

            return response;
        });
    }

    public async Task<Response> LessonRemove(Request<Lesson> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await LessonService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new LessonRemoved
                {
                    Id = request.Value.Id,
                    UnitId = request.Value.UnitId,
                });

            return response;
        });
    }

    public async Task<Response<Copy>> LessonCopy(Request<Copy> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await LessonService.Copy(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new LessonAdded
                {
                    Id = request.Value.NewId,
                });

            return response;
        });
    }

    public async Task<Response> LessonSaveOrder(Request<IList<Lesson>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await LessonService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new LessonsReordered
                {
                    UnitId = request.Value.First().UnitId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> LessonFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await LessonService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Named>>> LessonFetchAchievementNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await LessonService.FetchAchievementNames(request));
    }
}