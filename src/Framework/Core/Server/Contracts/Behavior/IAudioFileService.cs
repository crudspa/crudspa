namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IAudioFileService
{
    Task<Response<AudioFile?>> Fetch(Request<AudioFile> request);
    Task<Response<AudioFile?>> Add(Request<AudioFile> request);
    Task<Response> Save(Request<AudioFile> request);
    Task<Response> Remove(Request<AudioFile> request);
}