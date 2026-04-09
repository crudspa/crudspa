create proc [FrameworkJobs].[VideoFileSelectForOptimization] (
     @SessionId uniqueidentifier
) as

declare @now datetimeoffset(7) = sysdatetimeoffset()
declare @batchId uniqueidentifier = newid()

update [Framework].[VideoFile]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,OptimizedBatchId = null
where IsDeleted = 0
    and OptimizedStatus = 1
    and OptimizedBlobId is not null
    and OptimizedFormat is not null
    and OptimizedBatchId is not null
    and (
        isnull(Width, 0) <= 0
        or isnull(Height, 0) <= 0
        or PosterBlobId is null
        or PosterFormat is null
    )

update [Framework].[VideoFile]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,OptimizedBatchId = @batchId
where IsDeleted = 0
    and OptimizedBatchId is null
    and (
        OptimizedStatus = 0
        or (
            OptimizedStatus = 1
            and OptimizedBlobId is not null
            and OptimizedFormat is not null
            and (
                isnull(Width, 0) <= 0
                or isnull(Height, 0) <= 0
                or PosterBlobId is null
                or PosterFormat is null
            )
        )
    )

select
     videoFile.Id
    ,videoFile.BlobId
    ,videoFile.Name
    ,videoFile.Format
    ,videoFile.Width
    ,videoFile.Height
    ,videoFile.OptimizedStatus
    ,videoFile.OptimizedBlobId
    ,videoFile.OptimizedFormat
    ,videoFile.PosterBlobId
    ,videoFile.PosterFormat
from [Framework].[VideoFile-Active] videoFile
where OptimizedBatchId = @batchId