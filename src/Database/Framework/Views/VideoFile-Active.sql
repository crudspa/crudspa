create view [Framework].[VideoFile-Active] as

select videoFile.Id as Id
    ,videoFile.BlobId as BlobId
    ,videoFile.Name as Name
    ,videoFile.Format as Format
    ,videoFile.Width as Width
    ,videoFile.Height as Height
    ,videoFile.OptimizedStatus as OptimizedStatus
    ,videoFile.OptimizedBlobId as OptimizedBlobId
    ,videoFile.OptimizedFormat as OptimizedFormat
    ,videoFile.OptimizedBatchId as OptimizedBatchId
    ,videoFile.PosterBlobId as PosterBlobId
    ,videoFile.PosterFormat as PosterFormat
from [Framework].[VideoFile] videoFile
where 1=1
    and videoFile.IsDeleted = 0