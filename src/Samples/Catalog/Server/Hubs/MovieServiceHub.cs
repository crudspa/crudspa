using PermissionIds = Crudspa.Samples.Catalog.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub
{
    public async Task<Response<IList<Movie>>> MovieSearch(Request<MovieSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await MovieService.Search(request);
        });
    }

    public async Task<Response<Movie?>> MovieFetch(Request<Movie> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
            await MovieService.Fetch(request));
    }

    public async Task<Response<Movie?>> MovieAdd(Request<Movie> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            var response = await MovieService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Movies, new MovieAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> MovieSave(Request<Movie> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            var response = await MovieService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Movies, new MovieSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> MovieRemove(Request<Movie> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
        {
            var response = await MovieService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Movies, new MovieRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> MovieFetchGenreNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
            await MovieService.FetchGenreNames(request));
    }

    public async Task<Response<IList<Orderable>>> MovieFetchRatingNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Movies, async session =>
            await MovieService.FetchRatingNames(request));
    }
}