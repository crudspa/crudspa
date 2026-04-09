create proc [FrameworkJobs].[VideoFileSelectBeenOptimized] as

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
where videoFile.OptimizedStatus != 0