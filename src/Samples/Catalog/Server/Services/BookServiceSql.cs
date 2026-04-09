namespace Crudspa.Samples.Catalog.Server.Services;

public class BookServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
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

            var samplePdfFileResponse = await fileService.SavePdf(new(request.SessionId, book.SamplePdfFile));
                if (!samplePdfFileResponse.Ok)
                {
                    response.AddErrors(samplePdfFileResponse.Errors);
                    return null;
                }

                book.SamplePdfFile.Id = samplePdfFileResponse.Value.Id;

            var previewAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, book.PreviewAudioFileFile));
                if (!previewAudioFileFileResponse.Ok)
                {
                    response.AddErrors(previewAudioFileFileResponse.Errors);
                    return null;
                }

                book.PreviewAudioFileFile.Id = previewAudioFileFileResponse.Value.Id;

            book.Summary = htmlSanitizer.Sanitize(book.Summary);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await BookInsert.Execute(connection, transaction, request.SessionId, book);

                foreach (var bookEdition in book.BookEditions)
                {
                    bookEdition.BookId = id;
                    await BookEditionInsertByBatch.Execute(connection, transaction, request.SessionId, bookEdition);
                }

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

            var samplePdfFileResponse = await fileService.SavePdf(new(request.SessionId, book.SamplePdfFile), existing?.SamplePdfFile);
            if (!samplePdfFileResponse.Ok)
            {
                response.AddErrors(samplePdfFileResponse.Errors);
                return;
            }

            book.SamplePdfFile.Id = samplePdfFileResponse.Value.Id;

            var previewAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, book.PreviewAudioFileFile), existing?.PreviewAudioFileFile);
            if (!previewAudioFileFileResponse.Ok)
            {
                response.AddErrors(previewAudioFileFileResponse.Errors);
                return;
            }

            book.PreviewAudioFileFile.Id = previewAudioFileFileResponse.Value.Id;

            book.Summary = htmlSanitizer.Sanitize(book.Summary);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await BookUpdate.Execute(connection, transaction, request.SessionId, book);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.BookEditions,
                    book.BookEditions,
                    BookEditionInsertByBatch.Execute,
                    BookEditionUpdateByBatch.Execute,
                    BookEditionDeleteByBatch.Execute);

                book.BookEditions.EnsureOrder();
                await BookEditionUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, book.BookEditions);
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

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                foreach (var bookEdition in existing.BookEditions)
                    await BookEditionDeleteByBatch.Execute(connection, transaction, request.SessionId, bookEdition);

                await BookDelete.Execute(connection, transaction, request.SessionId, book);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchGenreNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GenreSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchTagNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await TagSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchFormatNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await FormatSelectOrderables.Execute(Connection, request.SessionId));
    }
}