using System.Collections.ObjectModel;
using Azure.Storage.Blobs;

namespace Crudspa.Framework.Core.Server.Services;

public class BlobServiceAzure : IBlobService
{
    private readonly ILogger<BlobServiceAzure> _logger;
    private readonly BlobContainerClient _container;

    public BlobServiceAzure(ILogger<BlobServiceAzure> logger, IServerConfigService configService)
    {
        _logger = logger;

        try
        {
            _container = new(configService.Fetch().StorageAccount, configService.Fetch().StorageContainer);
            _container.CreateIfNotExists();
        }
        catch (Exception ex)
        {
            const String message = "Could not connect to storage container.";
            _logger.LogCritical(ex, message);
            throw new(message, ex);
        }
    }

    public async Task<Boolean> Exists(Guid blobId)
    {
        try
        {
            var client = GetBlobClient(new Blob { Id = blobId });
            return await client.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob existence check failed. {blobId}", blobId);
            return false;
        }
    }

    public async Task<PagedBlobs> List(Paged paged)
    {
        try
        {
            var blobKeys = new ObservableCollection<String>();

            var resultSegment = _container.GetBlobsAsync().AsPages(paged.ContinuationToken, paged.PageSize);

            await foreach (var blobPage in resultSegment)
            {
                blobKeys.AddRange(blobPage.Values.Select(b => b.Name));

                return new()
                {
                    BlobKeys = blobKeys,
                    PageSize = paged.PageSize,
                    ContinuationToken = blobPage.ContinuationToken,
                };
            }

            return new()
            {
                PageSize = paged.PageSize,
                ContinuationToken = null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blobs could not be listed.");
            return new();
        }
    }

    public async Task<Stream?> TryFetchStream(Guid blobId)
    {
        try
        {
            var client = GetBlobClient(new Blob { Id = blobId });
            return await client.OpenReadAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be opened as a stream. {blobId}", blobId);
            return null;
        }
    }

    public async Task<Blob?> Fetch(Blob blob)
    {
        try
        {
            return await GetBlob(blob);
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

        throw new FileNotFoundException($"Blob not found. {blobId:D}");
    }

    public async Task<String> FetchText(Guid? blobId)
    {
        var blob = await Fetch(new() { Id = blobId });

        return blob?.Data is not null
            ? blob.Data!.FromBytes()
            : String.Empty;
    }

    public async Task Add(Blob blob)
    {
        try
        {
            if (blob.Data is null)
                throw new("Add() was called without any data.");

            var client = GetBlobClient(blob);
            await client.UploadAsync(new BinaryData(blob.Data), overwrite: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be added. {blobId}", blob.Id);
            throw;
        }
    }

    public async Task AddStream(Guid blobId, Stream stream)
    {
        try
        {
            var client = GetBlobClient(new Blob { Id = blobId });
            await client.UploadAsync(stream, overwrite: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be added. {blobId}", blobId);
            throw;
        }
    }

    public async Task AddText(Guid blobId, String text)
    {
        try
        {
            var client = GetBlobClient(new Blob { Id = blobId });
            var bytes = text.ToBytes();

            await client.UploadAsync(new BinaryData(bytes), overwrite: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Text blob could not be added. {blobId}", blobId);
            throw;
        }
    }

    public async Task Remove(Blob blob)
    {
        try
        {
            var client = GetBlobClient(blob);
            await client.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be removed. {blobId}", blob.Id);
        }
    }

    public async Task Remove(String key)
    {
        try
        {
            var client = GetBlobClient(key);
            await client.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be removed. {key}", key);
        }
    }

    public async Task<Guid?> Copy(Guid sourceBlobId)
    {
        try
        {
            var sourceClient = GetBlobClient(new Blob { Id = sourceBlobId });

            if (!await sourceClient.ExistsAsync())
            {
                _logger.LogWarning("Source blob does not exist. {sourceBlobId}", sourceBlobId);
                return null;
            }

            var blobId = Guid.NewGuid();
            var destinationClient = GetBlobClient(new Blob { Id = blobId });

            await destinationClient.StartCopyFromUriAsync(sourceClient.Uri);

            return blobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob could not be copied. {sourceBlobId}", sourceBlobId);
            return null;
        }
    }

    private async Task<Blob?> GetBlob(Blob blob)
    {
        try
        {
            var client = GetBlobClient(blob);

            await using var memoryStream = new MemoryStream();
            await client.DownloadToAsync(memoryStream);

            blob.Data = memoryStream.ToArray();

            return blob;
        }
        catch
        {
            //ignore
            return null;
        }
    }

    private BlobClient GetBlobClient(Blob blob) => _container.GetBlobClient($"{blob.Id:D}");
    private BlobClient GetBlobClient(String key) => _container.GetBlobClient(key);
}