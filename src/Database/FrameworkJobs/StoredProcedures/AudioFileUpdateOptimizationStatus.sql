create proc [FrameworkJobs].[AudioFileUpdateOptimizationStatus] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OptimizedStatus int
    ,@OptimizedBlobId uniqueidentifier
    ,@OptimizedFormat nvarchar(10)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[AudioFile]
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,OptimizedStatus = @OptimizedStatus
    ,OptimizedBlobId = @OptimizedBlobId
    ,OptimizedFormat = @OptimizedFormat
    ,OptimizedBatchId = iif(@OptimizedStatus = 0, null, OptimizedBatchId)
where Id = @Id