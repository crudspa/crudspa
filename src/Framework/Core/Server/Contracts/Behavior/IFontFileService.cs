namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IFontFileService
{
    Task<Response<FontFile?>> Fetch(Request<FontFile> request);
    Task<Response<FontFile?>> Add(Request<FontFile> request);
    Task<Response> Save(Request<FontFile> request);
    Task<Response> Remove(Request<FontFile> request);
}