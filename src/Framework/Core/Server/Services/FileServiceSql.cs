namespace Crudspa.Framework.Core.Server.Services;

public class FileServiceSql(
    IServiceWrappers wrappers,
    IBlobService blobService,
    IAudioFileService audioFileService,
    IFontFileService fontFileService,
    IImageFileService imageFileService,
    IPdfFileService pdfFileService,
    IVideoFileService videoFileService)
    : IFileService
{
    public async Task<Response<AudioFile?>> SaveAudio(Request<AudioFile> request, Guid? existingId)
    {
        var existing = await FetchExisting(request, existingId, audioFileService.Fetch, id => new() { Id = id });
        return await SaveAudio(request, existing);
    }

    public async Task<Response<AudioFile?>> SaveAudio(Request<AudioFile> request, AudioFile? existing = null)
    {
        return await SaveFile(request, existing,
            getBlobId: file => file.BlobId,
            setBlobId: (file, blobId) => file.BlobId = blobId,
            getId: file => file.Id,
            setId: (file, id) => file.Id = id,
            add: audioFileService.Add,
            save: audioFileService.Save,
            remove: audioFileService.Remove,
            prepareClone: PrepareAudioClone);
    }

    public async Task<Response<FontFile?>> SaveFont(Request<FontFile> request, Guid? existingId)
    {
        var existing = await FetchExisting(request, existingId, fontFileService.Fetch, id => new() { Id = id });
        return await SaveFont(request, existing);
    }

    public async Task<Response<FontFile?>> SaveFont(Request<FontFile> request, FontFile? existing = null)
    {
        return await SaveFile(request, existing,
            getBlobId: file => file.BlobId,
            setBlobId: (file, blobId) => file.BlobId = blobId,
            getId: file => file.Id,
            setId: (file, id) => file.Id = id,
            add: fontFileService.Add,
            save: fontFileService.Save,
            remove: fontFileService.Remove);
    }

    public async Task<Response<ImageFile?>> SaveImage(Request<ImageFile> request, Guid? existingId)
    {
        var existing = await FetchExisting(request, existingId, imageFileService.Fetch, id => new() { Id = id });
        return await SaveImage(request, existing);
    }

    public async Task<Response<ImageFile?>> SaveImage(Request<ImageFile> request, ImageFile? existing = null)
    {
        return await SaveFile(request, existing,
            getBlobId: file => file.BlobId,
            setBlobId: (file, blobId) => file.BlobId = blobId,
            getId: file => file.Id,
            setId: (file, id) => file.Id = id,
            add: imageFileService.Add,
            save: imageFileService.Save,
            remove: imageFileService.Remove,
            prepareClone: PrepareImageClone);
    }

    public async Task<Response<PdfFile?>> SavePdf(Request<PdfFile> request, Guid? existingId)
    {
        var existing = await FetchExisting(request, existingId, pdfFileService.Fetch, id => new() { Id = id });
        return await SavePdf(request, existing);
    }

    public async Task<Response<PdfFile?>> SavePdf(Request<PdfFile> request, PdfFile? existing = null)
    {
        return await SaveFile(request, existing,
            getBlobId: file => file.BlobId,
            setBlobId: (file, blobId) => file.BlobId = blobId,
            getId: file => file.Id,
            setId: (file, id) => file.Id = id,
            add: pdfFileService.Add,
            save: pdfFileService.Save,
            remove: pdfFileService.Remove);
    }

    public async Task<Response<VideoFile?>> SaveVideo(Request<VideoFile> request, Guid? existingId)
    {
        var existing = await FetchExisting(request, existingId, videoFileService.Fetch, id => new() { Id = id });
        return await SaveVideo(request, existing);
    }

    public async Task<Response<VideoFile?>> SaveVideo(Request<VideoFile> request, VideoFile? existing = null)
    {
        return await SaveFile(request, existing,
            getBlobId: file => file.BlobId,
            setBlobId: (file, blobId) => file.BlobId = blobId,
            getId: file => file.Id,
            setId: (file, id) => file.Id = id,
            add: videoFileService.Add,
            save: videoFileService.Save,
            remove: videoFileService.Remove,
            prepareClone: PrepareVideoClone);
    }

    private static async Task<TFile?> FetchExisting<TFile>(Request<TFile> request, Guid? existingId, Func<Request<TFile>, Task<Response<TFile?>>> fetch, Func<Guid, TFile> createFetchModel)
        where TFile : class
    {
        if (!existingId.HasValue)
            return null;

        var fetchResponse = await fetch(new(request.SessionId, createFetchModel(existingId.Value)));
        return fetchResponse.Ok ? fetchResponse.Value : null;
    }

    private async Task<Response<TFile?>> SaveFile<TFile>(
        Request<TFile> request,
        TFile? existing,
        Func<TFile, Guid?> getBlobId,
        Action<TFile, Guid?> setBlobId,
        Func<TFile, Guid?> getId,
        Action<TFile, Guid?> setId,
        Func<Request<TFile>, Task<Response<TFile?>>> add,
        Func<Request<TFile>, Task<Response>> save,
        Func<Request<TFile>, Task<Response>> remove,
        Action<TFile>? prepareClone = null)
        where TFile : class, new()
    {
        return await wrappers.Try<TFile?>(request, async response =>
        {
            var incoming = request.Value;
            var incomingBlobId = getBlobId(incoming);
            var incomingId = getId(incoming);
            var existingBlobId = existing is not null ? getBlobId(existing) : null;
            var existingId = existing is not null ? getId(existing) : null;

            if (!incomingBlobId.HasValue)
            {
                if (incomingId.HasValue)
                {
                    if (existingId.HasValue && existingId == incomingId)
                        setBlobId(incoming, existingBlobId);

                    return incoming;
                }

                if (existingBlobId.HasValue)
                    await remove(new(request.SessionId, existing!));

                return new();
            }

            if (!existingBlobId.HasValue)
            {
                var isCloning = getId(incoming).HasValue;
                if (isCloning)
                {
                    setBlobId(incoming, await CloneBlob(incomingBlobId));
                    prepareClone?.Invoke(incoming);
                    setId(incoming, null);
                }

                var addResponse = await add(request);
                if (!addResponse.Ok)
                {
                    response.AddErrors(addResponse.Errors);
                    return null;
                }

                setId(incoming, getId(addResponse.Value!));
                return incoming;
            }

            if (incomingBlobId == existingBlobId)
            {
                var saveResponse = await save(request);
                if (!saveResponse.Ok)
                {
                    response.AddErrors(saveResponse.Errors);
                    return null;
                }

                setId(incoming, getId(existing!));
                return incoming;
            }

            await remove(new(request.SessionId, existing!));

            var addNewResponse = await add(request);
            if (!addNewResponse.Ok)
            {
                response.AddErrors(addNewResponse.Errors);
                return null;
            }

            setId(incoming, getId(addNewResponse.Value!));
            return incoming;
        });
    }

    private static void PrepareAudioClone(AudioFile file)
    {
        file.OptimizedStatus = AudioFile.OptimizationStatus.None;
        file.OptimizedBlobId = null;
        file.OptimizedFormat = null;
    }

    private static void PrepareImageClone(ImageFile file)
    {
        file.OptimizedStatus = ImageFile.OptimizationStatus.None;
        file.OptimizedBlobId = null;
        file.OptimizedFormat = null;
        file.Resized96BlobId = null;
        file.Resized192BlobId = null;
        file.Resized360BlobId = null;
        file.Resized540BlobId = null;
        file.Resized720BlobId = null;
        file.Resized1080BlobId = null;
        file.Resized1440BlobId = null;
        file.Resized1920BlobId = null;
        file.Resized3840BlobId = null;
    }

    private static void PrepareVideoClone(VideoFile file)
    {
        file.OptimizedStatus = VideoFile.OptimizationStatus.None;
        file.OptimizedBlobId = null;
        file.OptimizedFormat = null;
    }

    private async Task<Guid?> CloneBlob(Guid? blobId)
    {
        if (!blobId.HasValue)
            return null;

        return await blobService.Copy(blobId.Value) ?? blobId;
    }
}