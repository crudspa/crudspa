create proc [FrameworkJobs].[VideoFileUpdateOptimizationStatus] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Width int
    ,@Height int
    ,@OptimizedStatus int
    ,@OptimizedBlobId uniqueidentifier
    ,@OptimizedFormat nvarchar(10)
    ,@PosterBlobId uniqueidentifier
    ,@PosterFormat nvarchar(10)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[VideoFile]
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Width = @Width
    ,Height = @Height
    ,OptimizedStatus = @OptimizedStatus
    ,OptimizedBlobId = @OptimizedBlobId
    ,OptimizedFormat = @OptimizedFormat
    ,PosterBlobId = @PosterBlobId
    ,PosterFormat = @PosterFormat
    ,OptimizedBatchId = null
where Id = @Id