using AchievementSelectNames = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelectNames;

namespace Crudspa.Education.Publisher.Server.Services;

public class ModuleServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IPagePartsService pagePartsService)
    : IModuleService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Module>>> FetchForBook(Request<Book> request)
    {
        return await wrappers.Try<IList<Module>>(request, async response =>
        {
            var modules = await ModuleSelectForBook.Execute(Connection, request.SessionId, request.Value.Id);

            return modules;
        });
    }

    public async Task<Response<Module?>> Fetch(Request<Module> request)
    {
        return await wrappers.Try<Module?>(request, async response =>
        {
            var module = await ModuleSelect.Execute(Connection, request.SessionId, request.Value);

            return module;
        });
    }

    public async Task<Response<Module?>> Add(Request<Module> request)
    {
        return await wrappers.Validate<Module?, Module>(request, async response =>
        {
            var module = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ModuleInsert.Execute(connection, transaction, request.SessionId, module);

                return new Module
                {
                    Id = id,
                    BookId = module.BookId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Module> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var module = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ModuleUpdate.Execute(connection, transaction, request.SessionId, module);
            });
        });
    }

    public async Task<Response> Remove(Request<Module> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var module = request.Value;
            var existing = await ModuleSelect.Execute(Connection, request.SessionId, module);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ModuleDelete.Execute(connection, transaction, request.SessionId, module);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Module>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var modules = request.Value;

            modules.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ModuleUpdateOrdinals.Execute(connection, transaction, request.SessionId, modules);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request)
    {
        return await wrappers.Try<IList<IconFull>>(request, async response =>
            await IconSelectFull.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AchievementSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BinderTypeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Page>>> FetchPages(Request<ModulePage> request)
    {
        return await wrappers.Try<IList<Page>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);

            if (binderId.HasNothing())
                throw new("Module binder not found.");

            var fetchResponse = await pagePartsService.FetchPages(request.SessionId, binderId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<ModulePage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Module page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, binderId, page);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> AddPage(Request<ModulePage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Module binder not found.");

            var addResponse = await pagePartsService.AddPage(request.SessionId, binderId, page);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SavePage(Request<ModulePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Module page not found.");

            var saveResponse = await pagePartsService.SavePage(request.SessionId, binderId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemovePage(Request<ModulePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Module page not found.");

            var removeResponse = await pagePartsService.RemovePage(request.SessionId, binderId, page);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SavePageOrder(Request<ModulePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pages = request.Value.Pages;

            if (!pages.HasItems() || binderId.HasNothing())
                throw new("Module binder not found.");

            var saveResponse = await pagePartsService.SavePageOrder(request.SessionId, binderId, pages);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<ModuleSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pageId = request.Value.PageId;

            if (pageId.HasNothing() || binderId.HasNothing())
                throw new("Module page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, binderId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<ModuleSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Module section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<ModuleSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Module page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response<Section?>> DuplicateSection(Request<ModuleSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Module section not found.");

            var duplicateResponse = await pagePartsService.DuplicateSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(duplicateResponse.Errors);
            return duplicateResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<ModuleSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Module section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<ModuleSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Module section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<ModuleSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ModuleId);
            var pageId = request.Value.PageId;
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing() || binderId.HasNothing())
                throw new("Module page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, binderId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchBinderId(Guid? sessionId, Guid? moduleId)
    {
        var module = await ModuleSelect.Execute(Connection, sessionId, new() { Id = moduleId });
        return module?.Binder?.Id;
    }
}