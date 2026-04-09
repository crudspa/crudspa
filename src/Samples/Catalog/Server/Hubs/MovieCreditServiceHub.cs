using PermissionIds = Crudspa.Samples.Catalog.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub
{
    public async Task<Response<IList<MovieCredit>>> MovieCreditFetchForMovie(Request<Movie> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
            await MovieCreditService.FetchForMovie(request));
    }

    public async Task<Response<MovieCredit?>> MovieCreditFetch(Request<MovieCredit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
            await MovieCreditService.Fetch(request));
    }

    public async Task<Response<MovieCredit?>> MovieCreditAdd(Request<MovieCredit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            var response = await MovieCreditService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Movies, new MovieCreditAdded
                {
                    Id = response.Value.Id,
                    MovieId = request.Value.MovieId,
                });

            return response;
        });
    }

    public async Task<Response> MovieCreditSave(Request<MovieCredit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            var response = await MovieCreditService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Movies, new MovieCreditSaved
                {
                    Id = request.Value.Id,
                    MovieId = request.Value.MovieId,
                });

            return response;
        });
    }

    public async Task<Response> MovieCreditRemove(Request<MovieCredit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            var response = await MovieCreditService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Movies, new MovieCreditRemoved
                {
                    Id = request.Value.Id,
                    MovieId = request.Value.MovieId,
                });

            return response;
        });
    }

    public async Task<Response> MovieCreditSaveOrder(Request<IList<MovieCredit>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            var response = await MovieCreditService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Movies, new MovieCreditsReordered
                {
                    MovieId = request.Value.First().MovieId,
                });

            return response;
        });
    }
}