namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IVideoFileService
{
    Task<Response<VideoFile?>> Fetch(Request<VideoFile> request);
    Task<Response<VideoFile?>> Add(Request<VideoFile> request);
    Task<Response> Save(Request<VideoFile> request);
    Task<Response> Remove(Request<VideoFile> request);
}