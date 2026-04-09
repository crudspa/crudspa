create proc [FrameworkCore].[AudioFileUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
    ,@OptimizedStatus int
    ,@OptimizedBlobId uniqueidentifier
    ,@OptimizedFormat nvarchar(10)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[AudioFile]
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,BlobId = @BlobId
    ,Name = @Name
    ,Format = @Format
    ,OptimizedStatus = @OptimizedStatus
    ,OptimizedBlobId = @OptimizedBlobId
    ,OptimizedFormat = @OptimizedFormat
    ,OptimizedBatchId = iif(@OptimizedStatus = 0, null, OptimizedBatchId)
where Id = @Id