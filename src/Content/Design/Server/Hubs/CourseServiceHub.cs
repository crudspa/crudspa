using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Course>>> CourseFetchForTrack(Request<Track> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchForTrack(request));
    }

    public async Task<Response<Course?>> CourseFetch(Request<Course> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.Fetch(request));
    }

    public async Task<Response<Course?>> CourseAdd(Request<Course> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new CourseAdded
                {
                    Id = response.Value.Id,
                    TrackId = request.Value.TrackId,
                });

            return response;
        });
    }

    public async Task<Response> CourseSave(Request<Course> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new CourseSaved
                {
                    Id = request.Value.Id,
                    TrackId = request.Value.TrackId,
                });

            return response;
        });
    }

    public async Task<Response> CourseRemove(Request<Course> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new CourseRemoved
                {
                    Id = request.Value.Id,
                    TrackId = request.Value.TrackId,
                });

            return response;
        });
    }

    public async Task<Response> CourseSaveOrder(Request<IList<Course>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new CoursesReordered
                {
                    TrackId = request.Value.First().TrackId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> CourseFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Named>>> CourseFetchAchievementNames(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchAchievementNames(request));
    }

    public async Task<Response<IList<Orderable>>> CourseFetchBinderTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchBinderTypeNames(request));
    }

    public async Task<Response<IList<Page>>> CourseFetchPages(Request<CoursePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchPages(request));
    }

    public async Task<Response<Page?>> CourseFetchPage(Request<CoursePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchPage(request));
    }

    public async Task<Response<Page?>> CourseAddPage(Request<CoursePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.AddPage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new PageAdded
                {
                    Id = response.Value.Id,
                    BinderId = response.Value.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> CourseSavePage(Request<CoursePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.SavePage(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Tracks, new PageSaved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Page?.Id });
            }

            return response;
        });
    }

    public async Task<Response> CourseRemovePage(Request<CoursePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.RemovePage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new PageRemoved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> CourseSavePageOrder(Request<CoursePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.SavePageOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new PagesReordered
                {
                    BinderId = request.Value.Pages.FirstOrDefault()?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Section>>> CourseFetchSections(Request<CourseSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchSections(request));
    }

    public async Task<Response<Section?>> CourseFetchSection(Request<CourseSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await CourseService.FetchSection(request));
    }

    public async Task<Response<Section?>> CourseAddSection(Request<CourseSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.AddSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Tracks, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> CourseSaveSection(Request<CourseSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.SaveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Tracks, new SectionSaved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> CourseRemoveSection(Request<CourseSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.RemoveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Tracks, new SectionRemoved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> CourseSaveSectionOrder(Request<CourseSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await CourseService.SaveSectionOrder(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Tracks, new SectionsReordered
                {
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }
}