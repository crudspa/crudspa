namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IFileService
{
    Task<Response<AudioFile?>> SaveAudio(Request<AudioFile> request, Guid? existingId);
    Task<Response<AudioFile?>> SaveAudio(Request<AudioFile> request, AudioFile? existing = null);
    Task<Response<FontFile?>> SaveFont(Request<FontFile> request, Guid? existingId);
    Task<Response<FontFile?>> SaveFont(Request<FontFile> request, FontFile? existing = null);
    Task<Response<ImageFile?>> SaveImage(Request<ImageFile> request, Guid? existingId);
    Task<Response<ImageFile?>> SaveImage(Request<ImageFile> request, ImageFile? existing = null);
    Task<Response<PdfFile?>> SavePdf(Request<PdfFile> request, Guid? existingId);
    Task<Response<PdfFile?>> SavePdf(Request<PdfFile> request, PdfFile? existing = null);
    Task<Response<VideoFile?>> SaveVideo(Request<VideoFile> request, Guid? existingId);
    Task<Response<VideoFile?>> SaveVideo(Request<VideoFile> request, VideoFile? existing = null);
}