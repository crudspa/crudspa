using AchievementSelectNames = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelectNames;

namespace Crudspa.Education.Publisher.Server.Services;

public class BookServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IPagePartsService pagePartsService)
    : IBookService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Book>>> Search(Request<BookSearch> request)
    {
        return await wrappers.Try<IList<Book>>(request, async response =>
        {
            var books = await BookSelectWhere.Execute(Connection, request.SessionId, request.Value);

            return books;
        });
    }

    public async Task<Response<Book?>> Fetch(Request<Book> request)
    {
        return await wrappers.Try<Book?>(request, async response =>
        {
            var book = await BookSelect.Execute(Connection, request.SessionId, request.Value);

            return book;
        });
    }

    public async Task<Response<Book?>> Add(Request<Book> request)
    {
        return await wrappers.Validate<Book?, Book>(request, async response =>
        {
            var book = request.Value;

            var coverImageFileResponse = await fileService.SaveImage(new(request.SessionId, book.CoverImageFile));
            if (!coverImageFileResponse.Ok)
            {
                response.AddErrors(coverImageFileResponse.Errors);
                return null;
            }

            book.CoverImageFile.Id = coverImageFileResponse.Value.Id;

            var guideImageFileResponse = await fileService.SaveImage(new(request.SessionId, book.GuideImageFile));
            if (!guideImageFileResponse.Ok)
            {
                response.AddErrors(guideImageFileResponse.Errors);
                return null;
            }

            book.GuideImageFile.Id = guideImageFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await BookInsert.Execute(connection, transaction, request.SessionId, book);

                return new Book
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Book> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var book = request.Value;

            var existing = await BookSelect.Execute(Connection, request.SessionId, book);

            var coverImageFileResponse = await fileService.SaveImage(new(request.SessionId, book.CoverImageFile), existing?.CoverImageFile);
            if (!coverImageFileResponse.Ok)
            {
                response.AddErrors(coverImageFileResponse.Errors);
                return;
            }

            book.CoverImageFile.Id = coverImageFileResponse.Value.Id;

            var guideImageFileResponse = await fileService.SaveImage(new(request.SessionId, book.GuideImageFile), existing?.GuideImageFile);
            if (!guideImageFileResponse.Ok)
            {
                response.AddErrors(guideImageFileResponse.Errors);
                return;
            }

            book.GuideImageFile.Id = guideImageFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BookUpdate.Execute(connection, transaction, request.SessionId, book);
            });
        });
    }

    public async Task<Response> Remove(Request<Book> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var book = request.Value;
            var existing = await BookSelect.Execute(Connection, request.SessionId, book);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BookDelete.Execute(connection, transaction, request.SessionId, book);
            });
        });
    }

    public async Task<Response<Book>> FetchRelations(Request<Book> request)
    {
        return await wrappers.Try<Book>(request, async response =>
        {
            var book = await BookSelectRelations.Execute(Connection, request.SessionId, request.Value);
            return book;
        });
    }

    public async Task<Response> SaveRelations(Request<Book> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var book = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BookUpdateRelations.Execute(connection, transaction, request.SessionId, book);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchBookSeasonNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BookSeasonSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchBookCategoryNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BookCategorySelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AchievementSelectNames.Execute(Connection));
    }

    public async Task<Response<Book?>> FetchPrefaceBinderId(Request<Book> request)
    {
        return await wrappers.Try<Book?>(request, async response =>
            await BookSelectPrefaceBinderId.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<IList<Page>>> FetchPages(Request<BookPage> request)
    {
        return await wrappers.Try<IList<Page>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);

            if (binderId.HasNothing())
                throw new("Book preface binder not found.");

            var fetchResponse = await pagePartsService.FetchPages(request.SessionId, binderId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<BookPage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Book page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, binderId, page);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> AddPage(Request<BookPage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Book preface binder not found.");

            var addResponse = await pagePartsService.AddPage(request.SessionId, binderId, page);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SavePage(Request<BookPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Book page not found.");

            var saveResponse = await pagePartsService.SavePage(request.SessionId, binderId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemovePage(Request<BookPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Book page not found.");

            var removeResponse = await pagePartsService.RemovePage(request.SessionId, binderId, page);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SavePageOrder(Request<BookPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pages = request.Value.Pages;

            if (!pages.HasItems() || binderId.HasNothing())
                throw new("Book preface binder not found.");

            var saveResponse = await pagePartsService.SavePageOrder(request.SessionId, binderId, pages);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<BookSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pageId = request.Value.PageId;

            if (pageId.HasNothing() || binderId.HasNothing())
                throw new("Book page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, binderId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<BookSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Book section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<BookSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Book page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response<Section?>> DuplicateSection(Request<BookSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Book section not found.");

            var duplicateResponse = await pagePartsService.DuplicateSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(duplicateResponse.Errors);
            return duplicateResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<BookSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Book section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<BookSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Book section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<BookSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.BookId);
            var pageId = request.Value.PageId;
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing() || binderId.HasNothing())
                throw new("Book page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, binderId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchBinderId(Guid? sessionId, Guid? bookId)
    {
        var book = await BookSelectPrefaceBinderId.Execute(Connection, sessionId, new() { Id = bookId });
        return book?.PrefaceBinderId;
    }
}