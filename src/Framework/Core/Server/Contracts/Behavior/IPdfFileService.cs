namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IPdfFileService
{
    Task<Response<PdfFile?>> FetchAndLog(Request<PdfFile> request);
    Task<Response<PdfFile?>> Fetch(Request<PdfFile> request);
    Task<Response<PdfFile?>> Add(Request<PdfFile> request);
    Task<Response> Save(Request<PdfFile> request);
    Task<Response> Remove(Request<PdfFile> request);
}