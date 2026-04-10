namespace Crudspa.Framework.Jobs.Shared.Contracts.Behavior;

public interface IFrameworkActionService
{
    Task<Response> ExpireSessions(Request request, Int32? sessionLengthInDays);
    Task<Response<IList<AudioFile>>> FetchAudioForOptimization(Request request);
    Task<Response<IList<AudioFile>>> FetchAudioBeenOptimized(Request request);
    Task<Response> SaveAudioOptimizationStatus(Request<AudioFile> request);
    Task<Response<IList<ImageFile>>> FetchImageForOptimization(Request request);
    Task<Response<IList<ImageFile>>> FetchImageBeenOptimized(Request request);
    Task<Response> SaveImageOptimizationStatus(Request<ImageFile> request);
    Task<Response<IList<ImageFile>>> FetchImageForCaptioning(Request request);
    Task<Response> SaveImageCaption(Request<ImageFile> request);
    Task<Response<IList<VideoFile>>> FetchVideoForOptimization(Request request);
    Task<Response<IList<VideoFile>>> FetchVideoBeenOptimized(Request request);
    Task<Response> SaveVideoOptimizationStatus(Request<VideoFile> request);
}