using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<GameActivity>>> GameActivityFetchForSection(Request<GameSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.FetchForSection(request));
    }

    public async Task<Response<GameActivity?>> GameActivityFetch(Request<GameActivity> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.Fetch(request));
    }

    public async Task<Response<GameActivity?>> GameActivityAdd(Request<GameActivity> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameActivityService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameActivityAdded
                {
                    Id = response.Value.Id,
                    SectionId = request.Value.SectionId,
                });

            return response;
        });
    }

    public async Task<Response> GameActivitySave(Request<GameActivity> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameActivityService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameActivitySaved
                {
                    Id = request.Value.Id,
                    SectionId = request.Value.SectionId,
                });

            return response;
        });
    }

    public async Task<Response> GameActivityRemove(Request<GameActivity> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameActivityService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameActivityRemoved
                {
                    Id = request.Value.Id,
                    SectionId = request.Value.SectionId,
                });

            return response;
        });
    }

    public async Task<Response> GameActivitySaveOrder(Request<IList<GameActivity>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameActivityService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameActivitiesReordered
                {
                    SectionId = request.Value.First().SectionId,
                });

            return response;
        });
    }

    public async Task<Response<IList<ActivityTypeFull>>> GameActivityFetchActivityTypes(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.FetchActivityTypes(request));
    }

    public async Task<Response<IList<Named>>> GameActivityFetchContentAreaNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.FetchContentAreaNames(request));
    }

    public async Task<Response<IList<Orderable>>> GameActivityFetchResearchGroupNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.FetchResearchGroupNames(request));
    }

    public async Task<Response<IList<Orderable>>> GameActivityFetchGameActivityTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.FetchGameActivityTypeNames(request));
    }

    public async Task<Response<IList<Named>>> GameActivityFetchBooks(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.FetchBooks(request));
    }

    public async Task<Response<IList<Named>>> GameActivityFetchSections(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameActivityService.FetchSections(request));
    }

    public async Task<Response> GameActivityShare(Request<GameActivityShare> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameActivityService.Share(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameActivityShared
                {
                    Id = request.Value.SourceGameActivityId,
                    SourceGameActivityId = request.Value.SourceGameActivityId,
                    TargetGameSectionId = request.Value.TargetGameSectionId,
                });

            return response;
        });
    }
}