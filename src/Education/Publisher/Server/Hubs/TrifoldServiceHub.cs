using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Trifold>>> TrifoldFetchForBook(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchForBook(request));
    }

    public async Task<Response<Trifold?>> TrifoldFetch(Request<Trifold> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.Fetch(request));
    }

    public async Task<Response<Trifold?>> TrifoldAdd(Request<Trifold> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new TrifoldAdded
                {
                    Id = response.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> TrifoldSave(Request<Trifold> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new TrifoldSaved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> TrifoldRemove(Request<Trifold> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new TrifoldRemoved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> TrifoldSaveOrder(Request<IList<Trifold>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new TrifoldsReordered
                {
                    BookId = request.Value.First().BookId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> TrifoldFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Named>>> TrifoldFetchAchievementNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchAchievementNames(request));
    }

    public async Task<Response<IList<Orderable>>> TrifoldFetchBinderTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchBinderTypeNames(request));
    }

    public async Task<Response<IList<Page>>> TrifoldFetchPages(Request<TrifoldPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchPages(request));
    }

    public async Task<Response<Page?>> TrifoldFetchPage(Request<TrifoldPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchPage(request));
    }

    public async Task<Response<Page?>> TrifoldAddPage(Request<TrifoldPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.AddPage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageAdded
                {
                    Id = response.Value.Id,
                    BinderId = response.Value.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> TrifoldSavePage(Request<TrifoldPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.SavePage(request);

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

    public async Task<Response> TrifoldRemovePage(Request<TrifoldPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.RemovePage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageRemoved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> TrifoldSavePageOrder(Request<TrifoldPage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.SavePageOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PagesReordered
                {
                    BinderId = request.Value.Pages.FirstOrDefault()?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Section>>> TrifoldFetchSections(Request<TrifoldSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchSections(request));
    }

    public async Task<Response<Section?>> TrifoldFetchSection(Request<TrifoldSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await TrifoldService.FetchSection(request));
    }

    public async Task<Response<Section?>> TrifoldAddSection(Request<TrifoldSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.AddSection(request);

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

    public async Task<Response> TrifoldSaveSection(Request<TrifoldSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.SaveSection(request);

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

    public async Task<Response> TrifoldRemoveSection(Request<TrifoldSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.RemoveSection(request);

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

    public async Task<Response> TrifoldSaveSectionOrder(Request<TrifoldSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await TrifoldService.SaveSectionOrder(request);

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