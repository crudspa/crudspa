using AchievementSelectNames = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelectNames;

namespace Crudspa.Education.Publisher.Server.Services;

public class TrifoldServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IPagePartsService pagePartsService)
    : ITrifoldService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Trifold>>> FetchForBook(Request<Book> request)
    {
        return await wrappers.Try<IList<Trifold>>(request, async response =>
        {
            var trifolds = await TrifoldSelectForBook.Execute(Connection, request.SessionId, request.Value.Id);

            return trifolds;
        });
    }

    public async Task<Response<Trifold?>> Fetch(Request<Trifold> request)
    {
        return await wrappers.Try<Trifold?>(request, async response =>
        {
            var trifold = await TrifoldSelect.Execute(Connection, request.SessionId, request.Value);

            return trifold;
        });
    }

    public async Task<Response<Trifold?>> Add(Request<Trifold> request)
    {
        return await wrappers.Validate<Trifold?, Trifold>(request, async response =>
        {
            var trifold = request.Value;

            var guideImageFileResponse = await fileService.SaveImage(new(request.SessionId, trifold.GuideBinder.GuideImage));
            if (!guideImageFileResponse.Ok)
            {
                response.AddErrors(guideImageFileResponse.Errors);
                return null;
            }

            trifold.GuideBinder.GuideImage.Id = guideImageFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await TrifoldInsert.Execute(connection, transaction, request.SessionId, trifold);
                var inserted = await TrifoldSelect.Execute(Connection, request.SessionId, new() { Id = id });

                trifold.GuideBinder.BinderId = inserted?.Binder.Id;
                trifold.GuideBinder.Id = await GuideBinderUpsert.Execute(connection, transaction, request.SessionId, trifold.GuideBinder);

                return new Trifold
                {
                    Id = id,
                    BookId = trifold.BookId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Trifold> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var trifold = request.Value;
            var existing = await TrifoldSelect.Execute(Connection, request.SessionId, trifold);

            var guideImageFileResponse = await fileService.SaveImage(new(request.SessionId, trifold.GuideBinder.GuideImage), existing?.GuideBinder.GuideImage);
            if (!guideImageFileResponse.Ok)
            {
                response.AddErrors(guideImageFileResponse.Errors);
                return;
            }

            trifold.GuideBinder.BinderId = existing?.Binder.Id;
            trifold.GuideBinder.GuideImage.Id = guideImageFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrifoldUpdate.Execute(connection, transaction, request.SessionId, trifold);
                trifold.GuideBinder.Id = await GuideBinderUpsert.Execute(connection, transaction, request.SessionId, trifold.GuideBinder);
            });
        });
    }

    public async Task<Response> Remove(Request<Trifold> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var trifold = request.Value;
            var existing = await TrifoldSelect.Execute(Connection, request.SessionId, trifold);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrifoldDelete.Execute(connection, transaction, request.SessionId, trifold);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Trifold>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var trifolds = request.Value;

            trifolds.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrifoldUpdateOrdinals.Execute(connection, transaction, request.SessionId, trifolds);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
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

    public async Task<Response<IList<Page>>> FetchPages(Request<TrifoldPage> request)
    {
        return await wrappers.Try<IList<Page>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);

            if (binderId.HasNothing())
                throw new("Trifold binder not found.");

            var fetchResponse = await pagePartsService.FetchPages(request.SessionId, binderId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<TrifoldPage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Trifold page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, binderId, page);
            response.AddErrors(fetchResponse.Errors);
            if (fetchResponse.Value is not null)
                await ApplyGuidePage(request.SessionId, fetchResponse.Value);

            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> AddPage(Request<TrifoldPage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Trifold binder not found.");

            var addResponse = await pagePartsService.AddPage(request.SessionId, binderId, page);
            response.AddErrors(addResponse.Errors);
            if (addResponse.Ok && addResponse.Value?.Id is not null)
            {
                page.Id = addResponse.Value.Id;
                await SaveGuidePage(request.SessionId, binderId, page);
            }

            return addResponse.Value!;
        });
    }

    public async Task<Response> SavePage(Request<TrifoldPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Trifold page not found.");

            await SaveGuidePage(request.SessionId, binderId, page);

            var saveResponse = await pagePartsService.SavePage(request.SessionId, binderId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemovePage(Request<TrifoldPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Trifold page not found.");

            var removeResponse = await pagePartsService.RemovePage(request.SessionId, binderId, page);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SavePageOrder(Request<TrifoldPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pages = request.Value.Pages;

            if (!pages.HasItems() || binderId.HasNothing())
                throw new("Trifold binder not found.");

            var saveResponse = await pagePartsService.SavePageOrder(request.SessionId, binderId, pages);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<TrifoldSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pageId = request.Value.PageId;

            if (pageId.HasNothing() || binderId.HasNothing())
                throw new("Trifold page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, binderId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<TrifoldSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Trifold section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<TrifoldSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Trifold page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response<Section?>> DuplicateSection(Request<TrifoldSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Trifold section not found.");

            var duplicateResponse = await pagePartsService.DuplicateSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(duplicateResponse.Errors);
            return duplicateResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<TrifoldSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Trifold section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<TrifoldSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Trifold section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<TrifoldSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.TrifoldId);
            var pageId = request.Value.PageId;
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing() || binderId.HasNothing())
                throw new("Trifold page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, binderId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchBinderId(Guid? sessionId, Guid? trifoldId)
    {
        var trifold = await TrifoldSelect.Execute(Connection, sessionId, new() { Id = trifoldId });
        return trifold?.Binder?.Id;
    }

    private async Task ApplyGuidePage(Guid? sessionId, Page page)
    {
        var guidePage = await GuidePageSelectForPage.Execute(Connection, sessionId, page.Id);

        page.ShowGuide = guidePage?.ShowGuide ?? false;
        page.GuideText = guidePage?.GuideText;
        page.GuideAudioFile = guidePage?.GuideAudioFile ?? new();
        page.ShowNotebook = guidePage?.ShowNotebook ?? false;
    }

    private async Task SaveGuidePage(Guid? sessionId, Guid? binderId, Page page)
    {
        var guideBinder = await GuideBinderSelectForBinder.Execute(Connection, sessionId, binderId) ?? new() { BinderId = binderId };
        var existing = await GuidePageSelectForPage.Execute(Connection, sessionId, page.Id);
        var guideAudioFileResponse = await fileService.SaveAudio(new(sessionId, page.GuideAudioFile), existing?.GuideAudioFile);

        if (!guideAudioFileResponse.Ok)
            throw new("Call to IFileService.SaveAudio() failed. " + guideAudioFileResponse.ErrorMessages);

        page.GuideAudioFile.Id = guideAudioFileResponse.Value.Id;

        await sqlWrappers.WithConnection(async (connection, transaction) =>
        {
            if (!guideBinder.Id.HasValue)
                guideBinder.Id = await GuideBinderUpsert.Execute(connection, transaction, sessionId, guideBinder);

            await GuidePageUpsert.Execute(connection, transaction, sessionId, new()
            {
                GuideBinderId = guideBinder.Id,
                PageId = page.Id,
                ShowGuide = page.ShowGuide ?? false,
                GuideText = page.GuideText,
                GuideAudioFile = page.GuideAudioFile,
                ShowNotebook = page.ShowNotebook ?? false,
            });
        });
    }
}