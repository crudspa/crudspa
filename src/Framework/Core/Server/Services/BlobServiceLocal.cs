namespace Crudspa.Framework.Core.Server.Services;

public class BlobServiceLocal : IBlobService
{
    private readonly ILogger<BlobServiceLocal> _logger;
    private readonly String _baseDirectory;

    public BlobServiceLocal(ILogger<BlobServiceLocal> logger, IServerConfigService configService, String? storageContainer = null)
    {
        _logger = logger;
        storageContainer ??= configService.Fetch().StorageContainer;

        _baseDirectory = Path.Combine(@"c:\data\temp\blobs\local", storageContainer);

        Directory.CreateDirectory(_baseDirectory);
    }

    public Task<Boolean> Exists(Guid blobId)
    {
        var filePath = TryGetFilePath(blobId);
        return Task.FromResult(filePath.HasSomething());
    }

    public Task<PagedBlobs> List(Paged paged)
    {
        try
        {
            var allFiles = FetchKeys();

            var pagedFiles = allFiles
                .Skip((paged.PageNumber - 1) * paged.PageSize)
                .Take(paged.PageSize)
                .ToList();

            var nextPageNumber = pagedFiles.Count == paged.PageSize && paged.PageNumber * paged.PageSize < allFiles.Count
                ? paged.PageNumber + 1
                : (Int32?)null;

            return Task.FromResult<PagedBlobs>(new()
            {
                BlobKeys = new(pagedFiles!),
                PageSize = paged.PageSize,
                ContinuationToken = nextPageNumber?.ToString(),
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blobs could not be listed.");
            return Task.FromResult<PagedBlobs>(new());
        }
    }

    public Task<Stream?> TryFetchStream(Guid blobId)
    {
        try
        {
            var filePath = TryGetFilePath(blobId);
            if (filePath.HasNothing())
                return Task.FromResult<Stream?>(null);

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<Stream?>(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be opened as a stream. {blobId}", blobId);
            return Task.FromResult<Stream?>(null);
        }
    }

    public async Task<Blob?> Fetch(Blob blob)
    {
        try
        {
            if (blob.Id is not { } blobId)
                return null;

            var filePath = TryGetFilePath(blobId);
            if (filePath.HasNothing())
                return null;

            blob.Data = await File.ReadAllBytesAsync(filePath);
            return blob;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be fetched. {blobId}", blob.Id);
            return null;
        }
    }

    public async Task<Stream> FetchStream(Guid blobId)
    {
        var stream = await TryFetchStream(blobId);

        if (stream is not null)
            return stream;

        throw new FileNotFoundException("Blob not found.", GetFilePath(blobId));
    }

    public async Task<String> FetchText(Guid? blobId)
    {
        try
        {
            if (blobId is null)
                return String.Empty;

            var filePath = TryGetFilePath(blobId.Value);
            if (filePath.HasNothing())
                return String.Empty;

            var bytes = await File.ReadAllBytesAsync(filePath);
            return bytes.FromBytes();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be fetched as text. {blobId}", blobId);
            return String.Empty;
        }
    }

    public async Task Add(Blob blob)
    {
        try
        {
            if (blob.Data is null)
                throw new ArgumentException("Add() was called without any data.");

            var filePath = GetFilePath(blob.Id!.Value);
            await File.WriteAllBytesAsync(filePath, blob.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be added. {blobId}", blob.Id);
        }
    }

    public async Task AddStream(Guid blobId, Stream stream)
    {
        try
        {
            var filePath = GetFilePath(blobId);
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be added. {blobId}", blobId);
        }
    }

    public async Task AddText(Guid blobId, String text)
    {
        try
        {
            var filePath = GetFilePath(blobId);
            var bytes = text.ToBytes();

            await File.WriteAllBytesAsync(filePath, bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Text blob could not be added. {blobId}", blobId);
        }
    }

    public Task Remove(Blob blob)
    {
        return Remove(blob.Id.ToString()!);
    }

    public Task Remove(String key)
    {
        try
        {
            var filePath = Path.Combine(_baseDirectory, key);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be removed. {key}", key);
        }

        return Task.CompletedTask;
    }

    public async Task<Guid?> Copy(Guid sourceBlobId)
    {
        try
        {
            var sourceFilePath = TryGetFilePath(sourceBlobId);
            if (sourceFilePath.HasNothing())
            {
                _logger.LogWarning("Source blob does not exist. {sourceBlobId}", sourceBlobId);
                return null;
            }

            var blobId = Guid.NewGuid();
            var destinationFilePath = GetFilePath(blobId);

            await Task.Run(() => File.Copy(sourceFilePath, destinationFilePath));

            return blobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be copied. {sourceBlobId}", sourceBlobId);
            return null;
        }
    }

    protected virtual IList<String> FetchKeys()
    {
        return Directory.GetFiles(_baseDirectory)
            .Select(Path.GetFileName)
            .Where(x => x.HasSomething())
            .Select(x => x!)
            .ToList();
    }

    protected virtual String? TryGetFilePath(Guid blobId)
    {
        var filePath = GetFilePath(blobId);
        return File.Exists(filePath)
            ? filePath
            : null;
    }

    protected String GetFilePath(Guid blobId)
    {
        return Path.Combine(_baseDirectory, $"{blobId:D}");
    }
}