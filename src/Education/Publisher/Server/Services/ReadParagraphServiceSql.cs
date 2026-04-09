namespace Crudspa.Education.Publisher.Server.Services;

public class ReadParagraphServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer)
    : IReadParagraphService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ReadParagraph>>> FetchForReadPart(Request<ReadPart> request)
    {
        return await wrappers.Try<IList<ReadParagraph>>(request, async response =>
        {
            var readParagraphs = await ReadParagraphSelectForReadPart.Execute(Connection, request.SessionId, request.Value.Id);

            return readParagraphs;
        });
    }

    public async Task<Response<ReadParagraph?>> Fetch(Request<ReadParagraph> request)
    {
        return await wrappers.Try<ReadParagraph?>(request, async response =>
        {
            var readParagraph = await ReadParagraphSelect.Execute(Connection, request.SessionId, request.Value);

            return readParagraph;
        });
    }

    public async Task<Response<ReadParagraph?>> Add(Request<ReadParagraph> request)
    {
        return await wrappers.Validate<ReadParagraph?, ReadParagraph>(request, async response =>
        {
            var readParagraph = request.Value;

            readParagraph.Text = htmlSanitizer.Sanitize(readParagraph.Text);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ReadParagraphInsert.Execute(connection, transaction, request.SessionId, readParagraph);

                return new ReadParagraph
                {
                    Id = id,
                    ReadPartId = readParagraph.ReadPartId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ReadParagraph> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var readParagraph = request.Value;

            readParagraph.Text = htmlSanitizer.Sanitize(readParagraph.Text);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadParagraphUpdate.Execute(connection, transaction, request.SessionId, readParagraph);
            });
        });
    }

    public async Task<Response> Remove(Request<ReadParagraph> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var readParagraph = request.Value;
            var existing = await ReadParagraphSelect.Execute(Connection, request.SessionId, readParagraph);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadParagraphDelete.Execute(connection, transaction, request.SessionId, readParagraph);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<ReadParagraph>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var readParagraphs = request.Value;

            readParagraphs.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadParagraphUpdateOrdinals.Execute(connection, transaction, request.SessionId, readParagraphs);
            });
        });
    }
}