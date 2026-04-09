namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IBlobService
{
    Task<Boolean> Exists(Guid blobId);
    Task<PagedBlobs> List(Paged paged);
    Task<Stream?> TryFetchStream(Guid blobId);
    Task<Blob?> Fetch(Blob blob);
    Task<Stream> FetchStream(Guid blobId);
    Task<String> FetchText(Guid? blobId);
    Task Add(Blob blob);
    Task AddStream(Guid blobId, Stream stream);
    Task AddText(Guid blobId, String text);
    Task Remove(Blob blob);
    Task Remove(String key);
    Task<Guid?> Copy(Guid sourceBlobId);
}