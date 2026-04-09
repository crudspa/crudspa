namespace Crudspa.Framework.Core.Server.Services;

public class AudioFileServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService)
    : IAudioFileService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<AudioFile?>> Fetch(Request<AudioFile> request)
    {
        return await wrappers.Try<AudioFile?>(request, async response =>
            await AudioFileSelect.Execute(Connection, request.Value));
    }

    public async Task<Response<AudioFile?>> Add(Request<AudioFile> request)
    {
        return await wrappers.Validate<AudioFile?, AudioFile>(request, async response =>
        {
            var audioFile = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                audioFile.Id = await AudioFileInsert.Execute(connection, transaction, request.SessionId, audioFile));

            return await AudioFileSelect.Execute(Connection, audioFile);
        });
    }

    public async Task<Response> Save(Request<AudioFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var audioFile = request.Value;

            var existingAudioFile = await AudioFileSelect.Execute(Connection, audioFile);

            if (existingAudioFile is not null)
            {
                if (audioFile.BlobId != existingAudioFile.BlobId)
                {
                    audioFile.OptimizedStatus = AudioFile.OptimizationStatus.None;
                    audioFile.OptimizedBlobId = null;
                    audioFile.OptimizedFormat = null;

                    await blobService.Remove(new Blob { Id = existingAudioFile.BlobId });

                    if (existingAudioFile.OptimizedBlobId is not null)
                        await blobService.Remove(new Blob { Id = existingAudioFile.OptimizedBlobId });
                }
                else
                {
                    audioFile.OptimizedStatus = existingAudioFile.OptimizedStatus;
                    audioFile.OptimizedBlobId = existingAudioFile.OptimizedBlobId;
                    audioFile.OptimizedFormat = existingAudioFile.OptimizedFormat;
                }
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await AudioFileUpdate.Execute(connection, transaction, request.SessionId, audioFile));
        });
    }

    public async Task<Response> Remove(Request<AudioFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var audioFile = request.Value;

            var existingAudioFile = await AudioFileSelect.Execute(Connection, audioFile);

            if (existingAudioFile is not null)
            {
                await blobService.Remove(new Blob { Id = existingAudioFile.BlobId });

                if (existingAudioFile.OptimizedBlobId is not null)
                    await blobService.Remove(new Blob { Id = existingAudioFile.OptimizedBlobId });
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await AudioFileDelete.Execute(connection, transaction, request.SessionId, audioFile));
        });
    }
}