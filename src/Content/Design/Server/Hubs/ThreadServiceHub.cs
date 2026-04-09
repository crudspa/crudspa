using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;
using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Thread>>> ThreadSearchForForum(Request<ThreadSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await ThreadService.SearchForForum(request);
        });
    }

    public async Task<Response<Thread?>> ThreadFetch(Request<Thread> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await ThreadService.Fetch(request));
    }

    public async Task<Response<Thread?>> ThreadAdd(Request<Thread> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await ThreadService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ThreadAdded
                {
                    Id = response.Value.Id,
                    ForumId = request.Value.ForumId,
                });

            return response;
        });
    }

    public async Task<Response> ThreadSave(Request<Thread> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await ThreadService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ThreadSaved
                {
                    Id = request.Value.Id,
                    ForumId = request.Value.ForumId,
                });

            return response;
        });
    }

    public async Task<Response> ThreadRemove(Request<Thread> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await ThreadService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ThreadRemoved
                {
                    Id = request.Value.Id,
                    ForumId = request.Value.ForumId,
                });

            return response;
        });
    }
}