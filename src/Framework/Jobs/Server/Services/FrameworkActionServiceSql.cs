using AudioFileSelect = Crudspa.Framework.Jobs.Server.Sproxies.AudioFileSelect;
using ImageFileSelect = Crudspa.Framework.Jobs.Server.Sproxies.ImageFileSelect;
using ImageFileUpdate = Crudspa.Framework.Jobs.Server.Sproxies.ImageFileUpdate;
using VideoFileSelect = Crudspa.Framework.Jobs.Server.Sproxies.VideoFileSelect;

namespace Crudspa.Framework.Jobs.Server.Services;

public class FrameworkActionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService)
    : IFrameworkActionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response> ExpireSessions(Request request, Int32? sessionLengthInDays)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await SessionEndExpired.Execute(connection, transaction, sessionLengthInDays));
        });
    }

    public async Task<Response<IList<AudioFile>>> FetchAudioForOptimization(Request request)
    {
        return await wrappers.Try<IList<AudioFile>>(request, async response =>
            await AudioFileSelectForOptimization.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<AudioFile>>> FetchAudioBeenOptimized(Request request)
    {
        return await wrappers.Try<IList<AudioFile>>(request, async response =>
            await AudioFileSelectBeenOptimized.Execute(Connection));
    }

    public async Task<Response> SaveAudioOptimizationStatus(Request<AudioFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var audioFile = request.Value;

            var existingAudioFile = await AudioFileSelect.Execute(Connection, audioFile);

            if (audioFile.OptimizedStatus != AudioFile.OptimizationStatus.Succeeded
                && existingAudioFile?.OptimizedBlobId is not null)
            {
                await blobService.Remove(new Blob { Id = existingAudioFile.OptimizedBlobId });
                audioFile.OptimizedBlobId = null;
                audioFile.OptimizedFormat = null;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await AudioFileUpdateOptimizationStatus.Execute(connection, transaction, request.SessionId, request.Value));
        });
    }

    public async Task<Response<IList<ImageFile>>> FetchImageForOptimization(Request request)
    {
        return await wrappers.Try<IList<ImageFile>>(request, async response =>
            await ImageFileSelectForOptimization.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<ImageFile>>> FetchImageBeenOptimized(Request request)
    {
        return await wrappers.Try<IList<ImageFile>>(request, async response =>
            await ImageFileSelectBeenOptimized.Execute(Connection));
    }

    public async Task<Response> SaveImageOptimizationStatus(Request<ImageFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var imageFile = request.Value;

            var existingImageFile = await ImageFileSelect.Execute(Connection, imageFile);

            if (existingImageFile is not null && imageFile.OptimizedStatus != ImageFile.OptimizationStatus.Succeeded)
            {
                if (existingImageFile.OptimizedBlobId is not null)
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.OptimizedBlobId });
                    imageFile.OptimizedBlobId = null;
                }

                if (existingImageFile.Resized96BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized96BlobId });
                    imageFile.Resized96BlobId = null;
                }

                if (existingImageFile.Resized192BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized192BlobId });
                    imageFile.Resized192BlobId = null;
                }

                if (existingImageFile.Resized360BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized360BlobId });
                    imageFile.Resized360BlobId = null;
                }

                if (existingImageFile.Resized540BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized540BlobId });
                    imageFile.Resized540BlobId = null;
                }

                if (existingImageFile.Resized720BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized720BlobId });
                    imageFile.Resized720BlobId = null;
                }

                if (existingImageFile.Resized1080BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized1080BlobId });
                    imageFile.Resized1080BlobId = null;
                }

                if (existingImageFile.Resized1440BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized1440BlobId });
                    imageFile.Resized1440BlobId = null;
                }

                if (existingImageFile.Resized1920BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized1920BlobId });
                    imageFile.Resized1920BlobId = null;
                }

                if (existingImageFile.Resized3840BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized3840BlobId });
                    imageFile.Resized3840BlobId = null;
                }
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await ImageFileUpdateOptimizationStatus.Execute(connection, transaction, request.SessionId, request.Value));
        });
    }

    public async Task<Response<IList<ImageFile>>> FetchImageForCaptioning(Request request)
    {
        return await wrappers.Try<IList<ImageFile>>(request, async response =>
            await ImageFileSelectForCaptioning.Execute(Connection, request.SessionId));
    }

    public async Task<Response> SaveImageCaption(Request<ImageFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var imageFile = request.Value;

            var existingImageFile = await ImageFileSelect.Execute(Connection, imageFile);

            if (existingImageFile is not null)
                if (imageFile.BlobId != existingImageFile.BlobId)
                    await blobService.Remove(new Blob { Id = existingImageFile.BlobId });

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await ImageFileUpdate.Execute(connection, transaction, request.SessionId, imageFile));
        });
    }

    public async Task<Response<IList<VideoFile>>> FetchVideoForOptimization(Request request)
    {
        return await wrappers.Try<IList<VideoFile>>(request, async response =>
            await VideoFileSelectForOptimization.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<VideoFile>>> FetchVideoBeenOptimized(Request request)
    {
        return await wrappers.Try<IList<VideoFile>>(request, async response =>
            await VideoFileSelectBeenOptimized.Execute(Connection));
    }

    public async Task<Response> SaveVideoOptimizationStatus(Request<VideoFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var videoFile = request.Value;

            var existingVideoFile = await VideoFileSelect.Execute(Connection, videoFile);

            if (existingVideoFile is not null)
            {
                if (videoFile.OptimizedStatus != VideoFile.OptimizationStatus.Succeeded)
                {
                    if (existingVideoFile.OptimizedBlobId is not null)
                        await blobService.Remove(new Blob { Id = existingVideoFile.OptimizedBlobId });

                    if (existingVideoFile.PosterBlobId is not null)
                        await blobService.Remove(new Blob { Id = existingVideoFile.PosterBlobId });

                    videoFile.OptimizedBlobId = null;
                    videoFile.OptimizedFormat = null;
                    videoFile.PosterBlobId = null;
                    videoFile.PosterFormat = null;
                }
                else
                {
                    if (existingVideoFile.OptimizedBlobId is not null
                        && existingVideoFile.OptimizedBlobId != videoFile.OptimizedBlobId)
                        await blobService.Remove(new Blob { Id = existingVideoFile.OptimizedBlobId });

                    if (existingVideoFile.PosterBlobId is not null
                        && existingVideoFile.PosterBlobId != videoFile.PosterBlobId)
                        await blobService.Remove(new Blob { Id = existingVideoFile.PosterBlobId });
                }
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await VideoFileUpdateOptimizationStatus.Execute(connection, transaction, request.SessionId, request.Value));
        });
    }

}