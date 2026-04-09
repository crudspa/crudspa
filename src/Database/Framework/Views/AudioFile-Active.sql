create view [Framework].[AudioFile-Active] as

select audioFile.Id as Id
    ,audioFile.BlobId as BlobId
    ,audioFile.Name as Name
    ,audioFile.Format as Format
    ,audioFile.OptimizedStatus as OptimizedStatus
    ,audioFile.OptimizedBlobId as OptimizedBlobId
    ,audioFile.OptimizedFormat as OptimizedFormat
    ,audioFile.OptimizedBatchId as OptimizedBatchId
from [Framework].[AudioFile] audioFile
where 1=1
    and audioFile.IsDeleted = 0