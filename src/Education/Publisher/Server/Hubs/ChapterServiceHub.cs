using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Orderable>>> ChapterFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Chapter>>> ChapterFetchForBook(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.FetchForBook(request));
    }

    public async Task<Response<Chapter?>> ChapterFetch(Request<Chapter> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.Fetch(request));
    }

    public async Task<Response<Chapter?>> ChapterAdd(Request<Chapter> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ChapterAdded
                {
                    Id = response.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> ChapterSave(Request<Chapter> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ChapterSaved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> ChapterRemove(Request<Chapter> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ChapterRemoved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> ChapterSaveOrder(Request<IList<Chapter>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ChaptersReordered
                {
                    BookId = request.Value.First().BookId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ChapterFetchBinderTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.FetchBinderTypeNames(request));
    }

    public async Task<Response<IList<Page>>> ChapterFetchPages(Request<ChapterPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.FetchPages(request));
    }

    public async Task<Response<Page?>> ChapterFetchPage(Request<ChapterPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.FetchPage(request));
    }

    public async Task<Response<Page?>> ChapterAddPage(Request<ChapterPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.AddPage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageAdded
                {
                    Id = response.Value.Id,
                    BinderId = response.Value.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> ChapterSavePage(Request<ChapterPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.SavePage(request);

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

    public async Task<Response> ChapterRemovePage(Request<ChapterPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.RemovePage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageRemoved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> ChapterSavePageOrder(Request<ChapterPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.SavePageOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PagesReordered
                {
                    BinderId = request.Value.Pages.FirstOrDefault()?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Section>>> ChapterFetchSections(Request<ChapterSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.FetchSections(request));
    }

    public async Task<Response<Section?>> ChapterFetchSection(Request<ChapterSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ChapterService.FetchSection(request));
    }

    public async Task<Response<Section?>> ChapterAddSection(Request<ChapterSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.AddSection(request);

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

    public async Task<Response> ChapterSaveSection(Request<ChapterSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.SaveSection(request);

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

    public async Task<Response> ChapterRemoveSection(Request<ChapterSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.RemoveSection(request);

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

    public async Task<Response> ChapterSaveSectionOrder(Request<ChapterSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ChapterService.SaveSectionOrder(request);

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