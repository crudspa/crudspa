namespace Crudspa.Framework.Core.Server.Services;

public class VideoFileServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService)
    : IVideoFileService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<VideoFile?>> Fetch(Request<VideoFile> request)
    {
        return await wrappers.Try<VideoFile?>(request, async response =>
            await VideoFileSelect.Execute(Connection, request.Value));
    }

    public async Task<Response<VideoFile?>> Add(Request<VideoFile> request)
    {
        return await wrappers.Validate<VideoFile?, VideoFile>(request, async response =>
        {
            var videoFile = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                videoFile.Id = await VideoFileInsert.Execute(connection, transaction, request.SessionId, videoFile));

            return await VideoFileSelect.Execute(Connection, videoFile);
        });
    }

    public async Task<Response> Save(Request<VideoFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var videoFile = request.Value;

            var existingVideoFile = await VideoFileSelect.Execute(Connection, videoFile);

            if (existingVideoFile is not null)
            {
                if (videoFile.BlobId != existingVideoFile.BlobId)
                {
                    videoFile.OptimizedStatus = VideoFile.OptimizationStatus.None;
                    videoFile.OptimizedBlobId = null;
                    videoFile.OptimizedFormat = null;
                    videoFile.PosterBlobId = null;
                    videoFile.PosterFormat = null;

                    await blobService.Remove(new Blob { Id = existingVideoFile.BlobId });

                    if (existingVideoFile.OptimizedBlobId is not null)
                        await blobService.Remove(new Blob { Id = existingVideoFile.OptimizedBlobId });

                    if (existingVideoFile.PosterBlobId is not null)
                        await blobService.Remove(new Blob { Id = existingVideoFile.PosterBlobId });
                }
                else
                {
                    videoFile.OptimizedStatus = existingVideoFile.OptimizedStatus;
                    videoFile.OptimizedBlobId = existingVideoFile.OptimizedBlobId;
                    videoFile.OptimizedFormat = existingVideoFile.OptimizedFormat;
                    videoFile.PosterBlobId = existingVideoFile.PosterBlobId;
                    videoFile.PosterFormat = existingVideoFile.PosterFormat;
                }
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await VideoFileUpdate.Execute(connection, transaction, request.SessionId, videoFile));
        });
    }

    public async Task<Response> Remove(Request<VideoFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var videoFile = request.Value;

            var existingVideoFile = await VideoFileSelect.Execute(Connection, videoFile);

            if (existingVideoFile is not null)
            {
                await blobService.Remove(new Blob { Id = existingVideoFile.BlobId });

                if (existingVideoFile.OptimizedBlobId is not null)
                    await blobService.Remove(new Blob { Id = existingVideoFile.OptimizedBlobId });

                if (existingVideoFile.PosterBlobId is not null)
                    await blobService.Remove(new Blob { Id = existingVideoFile.PosterBlobId });
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await VideoFileDelete.Execute(connection, transaction, request.SessionId, videoFile));
        });
    }
}