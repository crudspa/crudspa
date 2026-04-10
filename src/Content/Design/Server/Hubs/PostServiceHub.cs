using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Post>>> PostSearchForBlog(Request<PostSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await PostService.SearchForBlog(request);
        });
    }

    public async Task<Response<Post?>> PostFetch(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await PostService.Fetch(request));
    }

    public async Task<Response<Post?>> PostFetchPageId(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await PostService.FetchPageId(request));
    }

    public async Task<Response<Post?>> PostAdd(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Blogs, new PostAdded
                {
                    Id = response.Value.Id,
                    BlogId = request.Value.BlogId,
                });

            return response;
        });
    }

    public async Task<Response> PostSave(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Blogs, new PostSaved
                {
                    Id = request.Value.Id,
                    BlogId = request.Value.BlogId,
                });

            return response;
        });
    }

    public async Task<Response> PostRemove(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Blogs, new PostRemoved
                {
                    Id = request.Value.Id,
                    BlogId = request.Value.BlogId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> PostFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await PostService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Orderable>>> PostFetchPageTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await PostService.FetchPageTypeNames(request));
    }

    public async Task<Response<Page?>> PostFetchPage(Request<PostPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await PostService.FetchPage(request));
    }

    public async Task<Response> PostSavePage(Request<PostPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.SavePage(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Blogs, new PageSaved
                {
                    Id = request.Value.Page?.Id,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Page?.Id });
            }

            return response;
        });
    }

    public async Task<Response<IList<Section>>> PostFetchSections(Request<PostSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await PostService.FetchSections(request));
    }

    public async Task<Response<Section?>> PostFetchSection(Request<PostSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await PostService.FetchSection(request));
    }

    public async Task<Response<Section?>> PostAddSection(Request<PostSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.AddSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Blogs, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = response.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = response.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response<Section?>> PostDuplicateSection(Request<PostSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.DuplicateSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Blogs, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = response.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = response.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> PostSaveSection(Request<PostSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.SaveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Blogs, new SectionSaved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.Section?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Section?.PageId });
            }

            return response;
        });
    }

    public async Task<Response> PostRemoveSection(Request<PostSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.RemoveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Blogs, new SectionRemoved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.Section?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Section?.PageId });
            }

            return response;
        });
    }

    public async Task<Response> PostSaveSectionOrder(Request<PostSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
        {
            var response = await PostService.SaveSectionOrder(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Blogs, new SectionsReordered
                {
                    PageId = request.Value.Sections.FirstOrDefault()?.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Sections.FirstOrDefault()?.PageId });
            }

            return response;
        });
    }
}