create proc [FrameworkJobs].[ImageFileUpdateOptimizationStatus] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Width int
    ,@Height int
    ,@OptimizedStatus int
    ,@OptimizedBlobId uniqueidentifier
    ,@OptimizedFormat nvarchar(10)
    ,@Resized96BlobId uniqueidentifier
    ,@Resized192BlobId uniqueidentifier
    ,@Resized360BlobId uniqueidentifier
    ,@Resized540BlobId uniqueidentifier
    ,@Resized720BlobId uniqueidentifier
    ,@Resized1080BlobId uniqueidentifier
    ,@Resized1440BlobId uniqueidentifier
    ,@Resized1920BlobId uniqueidentifier
    ,@Resized3840BlobId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset();

update [Framework].[ImageFile]
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Width = @Width
    ,Height = @Height
    ,OptimizedStatus = @OptimizedStatus
    ,OptimizedBlobId = @OptimizedBlobId
    ,OptimizedFormat = @OptimizedFormat
    ,Resized96BlobId = @Resized96BlobId
    ,Resized192BlobId = @Resized192BlobId
    ,Resized360BlobId = @Resized360BlobId
    ,Resized540BlobId = @Resized540BlobId
    ,Resized720BlobId = @Resized720BlobId
    ,Resized1080BlobId = @Resized1080BlobId
    ,Resized1440BlobId = @Resized1440BlobId
    ,Resized1920BlobId = @Resized1920BlobId
    ,Resized3840BlobId = @Resized3840BlobId
    ,OptimizedBatchId = null
where Id = @Id