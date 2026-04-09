using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<GameSection>>> GameSectionFetchForGame(Request<Game> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameSectionService.FetchForGame(request));
    }

    public async Task<Response<GameSection?>> GameSectionFetch(Request<GameSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameSectionService.Fetch(request));
    }

    public async Task<Response<GameSection?>> GameSectionAdd(Request<GameSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameSectionService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameSectionAdded
                {
                    Id = response.Value.Id,
                    GameId = request.Value.GameId,
                });

            return response;
        });
    }

    public async Task<Response> GameSectionSave(Request<GameSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameSectionService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameSectionSaved
                {
                    Id = request.Value.Id,
                    GameId = request.Value.GameId,
                });

            return response;
        });
    }

    public async Task<Response> GameSectionRemove(Request<GameSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameSectionService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameSectionRemoved
                {
                    Id = request.Value.Id,
                    GameId = request.Value.GameId,
                });

            return response;
        });
    }

    public async Task<Response> GameSectionSaveOrder(Request<IList<GameSection>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await GameSectionService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new GameSectionsReordered
                {
                    GameId = request.Value.First().GameId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> GameSectionFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameSectionService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Orderable>>> GameSectionFetchGameSectionTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await GameSectionService.FetchGameSectionTypeNames(request));
    }
}