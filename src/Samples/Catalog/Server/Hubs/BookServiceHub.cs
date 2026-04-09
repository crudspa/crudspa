using PermissionIds = Crudspa.Samples.Catalog.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub
{
    public async Task<Response<IList<Book>>> BookSearch(Request<BookSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await BookService.Search(request);
        });
    }

    public async Task<Response<Book?>> BookFetch(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.Fetch(request));
    }

    public async Task<Response<Book?>> BookAdd(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new BookAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> BookSave(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new BookSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> BookRemove(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new BookRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> BookFetchGenreNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchGenreNames(request));
    }

    public async Task<Response<IList<Orderable>>> BookFetchTagNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchTagNames(request));
    }

    public async Task<Response<IList<Orderable>>> BookFetchFormatNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchFormatNames(request));
    }
}