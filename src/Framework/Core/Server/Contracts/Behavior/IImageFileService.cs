namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IImageFileService
{
    Task<Response<ImageFile?>> Fetch(Request<ImageFile> request);
    Task<Response<ImageFile?>> Add(Request<ImageFile> request);
    Task<Response> Save(Request<ImageFile> request);
    Task<Response> Remove(Request<ImageFile> request);
}