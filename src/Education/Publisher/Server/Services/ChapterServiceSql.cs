namespace Crudspa.Education.Publisher.Server.Services;

public class ChapterServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IPagePartsService pagePartsService)
    : IChapterService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Chapter>>> FetchForBook(Request<Book> request)
    {
        return await wrappers.Try<IList<Chapter>>(request, async response =>
        {
            var chapters = await ChapterSelectForBook.Execute(Connection, request.SessionId, request.Value.Id);
            return chapters;
        });
    }

    public async Task<Response<Chapter?>> Fetch(Request<Chapter> request)
    {
        return await wrappers.Try<Chapter?>(request, async response =>
        {
            var chapter = await ChapterSelect.Execute(Connection, request.SessionId, request.Value);
            return chapter;
        });
    }

    public async Task<Response<Chapter?>> Add(Request<Chapter> request)
    {
        return await wrappers.Validate<Chapter?, Chapter>(request, async response =>
        {
            var chapter = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ChapterInsert.Execute(connection, transaction, request.SessionId, chapter);

                return new Chapter
                {
                    Id = id,
                    BookId = chapter.BookId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Chapter> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var chapter = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ChapterUpdate.Execute(connection, transaction, request.SessionId, chapter);
            });
        });
    }

    public async Task<Response> Remove(Request<Chapter> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var chapter = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ChapterDelete.Execute(connection, transaction, request.SessionId, chapter);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Chapter>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var chapters = request.Value;

            chapters.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ChapterUpdateOrdinals.Execute(connection, transaction, request.SessionId, chapters);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BinderTypeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Page>>> FetchPages(Request<ChapterPage> request)
    {
        return await wrappers.Try<IList<Page>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);

            if (binderId.HasNothing())
                throw new("Chapter binder not found.");

            var fetchResponse = await pagePartsService.FetchPages(request.SessionId, binderId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<ChapterPage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Chapter page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, binderId, page);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> AddPage(Request<ChapterPage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Chapter binder not found.");

            var addResponse = await pagePartsService.AddPage(request.SessionId, binderId, page);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SavePage(Request<ChapterPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Chapter page not found.");

            var saveResponse = await pagePartsService.SavePage(request.SessionId, binderId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemovePage(Request<ChapterPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Chapter page not found.");

            var removeResponse = await pagePartsService.RemovePage(request.SessionId, binderId, page);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SavePageOrder(Request<ChapterPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var pages = request.Value.Pages;

            if (!pages.HasItems() || binderId.HasNothing())
                throw new("Chapter binder not found.");

            var saveResponse = await pagePartsService.SavePageOrder(request.SessionId, binderId, pages);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<ChapterSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var pageId = request.Value.PageId;

            if (pageId.HasNothing() || binderId.HasNothing())
                throw new("Chapter page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, binderId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<ChapterSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Chapter section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<ChapterSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Chapter page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<ChapterSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Chapter section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<ChapterSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Chapter section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<ChapterSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ChapterId);
            var pageId = request.Value.PageId;
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing() || binderId.HasNothing())
                throw new("Chapter page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, binderId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchBinderId(Guid? sessionId, Guid? chapterId)
    {
        var chapter = await ChapterSelect.Execute(Connection, sessionId, new() { Id = chapterId });
        return chapter?.Binder?.Id;
    }
}