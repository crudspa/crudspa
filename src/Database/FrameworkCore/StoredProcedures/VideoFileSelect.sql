create proc [FrameworkCore].[VideoFileSelect] (
     @Id uniqueidentifier
) as

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
where videoFile.Id = @Id