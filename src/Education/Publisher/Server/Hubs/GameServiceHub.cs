using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Game>>> GameSearchForBook(Request<GameSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await GameService.SearchForBook(request);
        });
    }

    public async Task<Response<Game?>> GameFetch(Request<Game> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameService.Fetch(request));
    }

    public async Task<Response<Game?>> GameAdd(Request<Game> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameAdded
                {
                    Id = response.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> GameSave(Request<Game> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameSaved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> GameRemove(Request<Game> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameRemoved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> GameFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<IconFull>>> GameFetchIcons(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameService.FetchIcons(request));
    }

    public async Task<Response<IList<Orderable>>> GameFetchGradeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameService.FetchGradeNames(request));
    }

    public async Task<Response<IList<Orderable>>> GameFetchAssessmentLevelNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameService.FetchAssessmentLevelNames(request));
    }

    public async Task<Response<IList<Named>>> GameFetchAchievementNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameService.FetchAchievementNames(request));
    }
}