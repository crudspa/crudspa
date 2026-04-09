create proc [FrameworkJobs].[ImageFileSelectBeenOptimized] as

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
    ,imageFile.Resized96BlobId
    ,imageFile.Resized192BlobId
    ,imageFile.Resized360BlobId
    ,imageFile.Resized540BlobId
    ,imageFile.Resized720BlobId
    ,imageFile.Resized1080BlobId
    ,imageFile.Resized1440BlobId
    ,imageFile.Resized1920BlobId
    ,imageFile.Resized3840BlobId
from [Framework].[ImageFile-Active] imageFile
where imageFile.OptimizedStatus != 0