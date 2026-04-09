create proc [FrameworkJobs].[AudioFileSelectForOptimization] (
     @SessionId uniqueidentifier
) as

declare @now datetimeoffset(7) = sysdatetimeoffset()
declare @batchId uniqueidentifier = newid()

update [Framework].[AudioFile]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,OptimizedBatchId = @batchId
where IsDeleted = 0
    and OptimizedStatus = 0
    and OptimizedBatchId is null

select
     audioFile.Id
    ,audioFile.BlobId
    ,audioFile.Name
    ,audioFile.Format
    ,audioFile.OptimizedStatus
    ,audioFile.OptimizedBlobId
    ,audioFile.OptimizedFormat
from [Framework].[AudioFile-Active] audioFile
where OptimizedBatchId = @batchId