create proc [FrameworkJobs].[ImageFileSelectForCaptioning] (
     @SessionId uniqueidentifier
) as

select
     imageFile.Id
    ,imageFile.BlobId
    ,imageFile.Name
    ,imageFile.Format
    ,imageFile.Width
    ,imageFile.Height
    ,imageFile.Caption
    ,imageFile.OptimizedStatus
    ,imageFile.OptimizedBlobId
    ,imageFile.OptimizedFormat
from [Framework].[ImageFile-Active] imageFile
where imageFile.Caption is null
    and imageFile.OptimizedStatus = 1
    and imageFile.OptimizedBlobId is not null