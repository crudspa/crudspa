using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Objective>>> ObjectiveFetchForLesson(Request<Lesson> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchForLesson(request));
    }

    public async Task<Response<Objective?>> ObjectiveFetch(Request<Objective> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.Fetch(request));
    }

    public async Task<Response<Objective?>> ObjectiveAdd(Request<Objective> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new ObjectiveAdded
                {
                    Id = response.Value.Id,
                    LessonId = request.Value.LessonId,
                });

            return response;
        });
    }

    public async Task<Response> ObjectiveSave(Request<Objective> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new ObjectiveSaved
                {
                    Id = request.Value.Id,
                    LessonId = request.Value.LessonId,
                });

            return response;
        });
    }

    public async Task<Response> ObjectiveRemove(Request<Objective> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new ObjectiveRemoved
                {
                    Id = request.Value.Id,
                    LessonId = request.Value.LessonId,
                });

            return response;
        });
    }

    public async Task<Response> ObjectiveSaveOrder(Request<IList<Objective>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new ObjectivesReordered
                {
                    LessonId = request.Value.First().LessonId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ObjectiveFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Named>>> ObjectiveFetchAchievementNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchAchievementNames(request));
    }

    public async Task<Response<IList<Orderable>>> ObjectiveFetchBinderTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchBinderTypeNames(request));
    }

    public async Task<Response<Copy>> ObjectiveCopy(Request<Copy> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.Copy(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new ObjectiveAdded
                {
                    Id = request.Value.NewId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Page>>> ObjectiveFetchPages(Request<ObjectivePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchPages(request));
    }

    public async Task<Response<Page?>> ObjectiveFetchPage(Request<ObjectivePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchPage(request));
    }

    public async Task<Response<Page?>> ObjectiveAddPage(Request<ObjectivePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.AddPage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new PageAdded
                {
                    Id = response.Value.Id,
                    BinderId = response.Value.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> ObjectiveSavePage(Request<ObjectivePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.SavePage(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Units, new PageSaved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Page?.Id });
            }

            return response;
        });
    }

    public async Task<Response> ObjectiveRemovePage(Request<ObjectivePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.RemovePage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new PageRemoved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> ObjectiveSavePageOrder(Request<ObjectivePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.SavePageOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new PagesReordered
                {
                    BinderId = request.Value.Pages.FirstOrDefault()?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Section>>> ObjectiveFetchSections(Request<ObjectiveSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchSections(request));
    }

    public async Task<Response<Section?>> ObjectiveFetchSection(Request<ObjectiveSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await ObjectiveService.FetchSection(request));
    }

    public async Task<Response<Section?>> ObjectiveAddSection(Request<ObjectiveSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.AddSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Units, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response<Section?>> ObjectiveDuplicateSection(Request<ObjectiveSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.DuplicateSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Units, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = response.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = response.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> ObjectiveSaveSection(Request<ObjectiveSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.SaveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Units, new SectionSaved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> ObjectiveRemoveSection(Request<ObjectiveSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.RemoveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Units, new SectionRemoved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> ObjectiveSaveSectionOrder(Request<ObjectiveSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await ObjectiveService.SaveSectionOrder(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Units, new SectionsReordered
                {
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }
}