using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Module>>> ModuleFetchForBook(Request<Book> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchForBook(request));
    }

    public async Task<Response<Module?>> ModuleFetch(Request<Module> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.Fetch(request));
    }

    public async Task<Response<Module?>> ModuleAdd(Request<Module> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ModuleAdded
                {
                    Id = response.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> ModuleSave(Request<Module> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ModuleSaved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> ModuleRemove(Request<Module> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ModuleRemoved
                {
                    Id = request.Value.Id,
                    BookId = request.Value.BookId,
                });

            return response;
        });
    }

    public async Task<Response> ModuleSaveOrder(Request<IList<Module>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new ModulesReordered
                {
                    BookId = request.Value.First().BookId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ModuleFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<IconFull>>> ModuleFetchIcons(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchIcons(request));
    }

    public async Task<Response<IList<Named>>> ModuleFetchAchievementNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchAchievementNames(request));
    }

    public async Task<Response<IList<Orderable>>> ModuleFetchBinderTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchBinderTypeNames(request));
    }

    public async Task<Response<IList<Page>>> ModuleFetchPages(Request<ModulePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchPages(request));
    }

    public async Task<Response<Page?>> ModuleFetchPage(Request<ModulePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchPage(request));
    }

    public async Task<Response<Page?>> ModuleAddPage(Request<ModulePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.AddPage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageAdded
                {
                    Id = response.Value.Id,
                    BinderId = response.Value.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> ModuleSavePage(Request<ModulePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.SavePage(request);

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

    public async Task<Response> ModuleRemovePage(Request<ModulePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.RemovePage(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PageRemoved
                {
                    Id = request.Value.Page?.Id,
                    BinderId = request.Value.Page?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response> ModuleSavePageOrder(Request<ModulePage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.SavePageOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Books, new PagesReordered
                {
                    BinderId = request.Value.Pages.FirstOrDefault()?.BinderId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Section>>> ModuleFetchSections(Request<ModuleSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchSections(request));
    }

    public async Task<Response<Section?>> ModuleFetchSection(Request<ModuleSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
            await ModuleService.FetchSection(request));
    }

    public async Task<Response<Section?>> ModuleAddSection(Request<ModuleSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.AddSection(request);

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

    public async Task<Response> ModuleSaveSection(Request<ModuleSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.SaveSection(request);

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

    public async Task<Response> ModuleRemoveSection(Request<ModuleSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.RemoveSection(request);

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

    public async Task<Response> ModuleSaveSectionOrder(Request<ModuleSection> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Books, async session =>
        {
            var response = await ModuleService.SaveSectionOrder(request);

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