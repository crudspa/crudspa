create proc [FrameworkCore].[AudioFileSelect] (
     @Id uniqueidentifier
) as

select
    audioFile.Id
    ,audioFile.BlobId
    ,audioFile.Name
    ,audioFile.Format
    ,audioFile.OptimizedStatus
    ,audioFile.OptimizedBlobId
    ,audioFile.OptimizedFormat
from [Framework].[AudioFile-Active] audioFile
where audioFile.Id = @Id