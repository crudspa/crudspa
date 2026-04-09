namespace Crudspa.Framework.Core.Server.Services;

public class PdfFileServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService)
    : IPdfFileService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<PdfFile?>> FetchAndLog(Request<PdfFile> request)
    {
        return await wrappers.Try<PdfFile?>(request, async response =>
            await PdfFileSelectAndLog.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<PdfFile?>> Fetch(Request<PdfFile> request)
    {
        return await wrappers.Try<PdfFile?>(request, async response =>
            await PdfFileSelect.Execute(Connection, request.Value));
    }

    public async Task<Response<PdfFile?>> Add(Request<PdfFile> request)
    {
        return await wrappers.Try<PdfFile?>(request, async response =>
        {
            var pdfFile = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                pdfFile.Id = await PdfFileInsert.Execute(connection, transaction, request.SessionId, pdfFile));

            return await PdfFileSelect.Execute(Connection, pdfFile);
        });
    }

    public async Task<Response> Save(Request<PdfFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pdfFile = request.Value;

            var existingPdfFile = await PdfFileSelect.Execute(Connection, pdfFile);

            if (existingPdfFile is not null)
                if (pdfFile.BlobId != existingPdfFile.BlobId)
                    await blobService.Remove(new Blob { Id = existingPdfFile.BlobId });

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await PdfFileUpdate.Execute(connection, transaction, request.SessionId, pdfFile));
        });
    }

    public async Task<Response> Remove(Request<PdfFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pdfFile = request.Value;

            var existingPdfFile = await PdfFileSelect.Execute(Connection, pdfFile);

            if (existingPdfFile is not null)
                await blobService.Remove(new Blob { Id = existingPdfFile.BlobId });

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await PdfFileDelete.Execute(connection, transaction, request.SessionId, pdfFile));
        });
    }
}