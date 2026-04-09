using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Blog>>> BlogFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await BlogService.FetchForPortal(request));
    }

    public async Task<Response<Blog?>> BlogFetch(Request<Blog> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await BlogService.Fetch(request));
    }

    public async Task<Response<Blog?>> BlogAdd(Request<Blog> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await BlogService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Blogs, new BlogAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> BlogSave(Request<Blog> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await BlogService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Blogs, new BlogSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> BlogRemove(Request<Blog> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await BlogService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Blogs, new BlogRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> BlogFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await BlogService.FetchContentStatusNames(request));
    }
}