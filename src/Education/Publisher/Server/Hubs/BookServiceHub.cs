using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Orderable>>> BookFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchContentStatusNames(request));
    }

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

    public async Task<Response<Book>> BookFetchRelations(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchRelations(request));
    }

    public async Task<Response> BookSaveRelations(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.SaveRelations(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new BookRelationsSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> BookFetchBookSeasonNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchBookSeasonNames(request));
    }

    public async Task<Response<IList<Orderable>>> BookFetchBookCategoryNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchBookCategoryNames(request));
    }

    public async Task<Response<IList<Named>>> BookFetchAchievementNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchAchievementNames(request));
    }

    public async Task<Response<Book?>> BookFetchPrefaceBinderId(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchPrefaceBinderId(request));
    }

    public async Task<Response<IList<Page>>> BookFetchPages(Request<BookPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchPages(request));
    }

    public async Task<Response<Page?>> BookFetchPage(Request<BookPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchPage(request));
    }

    public async Task<Response<Page?>> BookAddPage(Request<BookPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.AddPage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageAdded
                {
                    Id = response.Value.Id,
                    BinderId = response.Value.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> BookSavePage(Request<BookPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.SavePage(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Books, new PageSaved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.Page?.Id });
            }

            return response;
        });
    }

    public async Task<Response> BookRemovePage(Request<BookPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.RemovePage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageRemoved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> BookSavePageOrder(Request<BookPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.SavePageOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PagesReordered
                {
                    BinderId = request.Value.Pages.FirstOrDefault()?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Section>>> BookFetchSections(Request<BookSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchSections(request));
    }

    public async Task<Response<Section?>> BookFetchSection(Request<BookSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await BookService.FetchSection(request));
    }

    public async Task<Response<Section?>> BookAddSection(Request<BookSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.AddSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Books, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response<Section?>> BookDuplicateSection(Request<BookSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.DuplicateSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Books, new SectionAdded
                {
                    Id = response.Value.Id,
                    PageId = response.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = response.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> BookSaveSection(Request<BookSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.SaveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Books, new SectionSaved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> BookRemoveSection(Request<BookSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.RemoveSection(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Books, new SectionRemoved
                {
                    Id = request.Value.Section?.Id,
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }

    public async Task<Response> BookSaveSectionOrder(Request<BookSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await BookService.SaveSectionOrder(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Books, new SectionsReordered
                {
                    PageId = request.Value.PageId,
                });

                await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
            }

            return response;
        });
    }
}