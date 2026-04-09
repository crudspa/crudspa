create view [Framework].[ImageFile-Active] as

select imageFile.Id as Id
    ,imageFile.BlobId as BlobId
    ,imageFile.Name as Name
    ,imageFile.Format as Format
    ,imageFile.Width as Width
    ,imageFile.Height as Height
    ,imageFile.Caption as Caption
    ,imageFile.OptimizedStatus as OptimizedStatus
    ,imageFile.OptimizedBlobId as OptimizedBlobId
    ,imageFile.OptimizedFormat as OptimizedFormat
    ,imageFile.OptimizedBatchId as OptimizedBatchId
    ,imageFile.Resized96BlobId as Resized96BlobId
    ,imageFile.Resized192BlobId as Resized192BlobId
    ,imageFile.Resized360BlobId as Resized360BlobId
    ,imageFile.Resized540BlobId as Resized540BlobId
    ,imageFile.Resized720BlobId as Resized720BlobId
    ,imageFile.Resized1080BlobId as Resized1080BlobId
    ,imageFile.Resized1440BlobId as Resized1440BlobId
    ,imageFile.Resized1920BlobId as Resized1920BlobId
    ,imageFile.Resized3840BlobId as Resized3840BlobId
from [Framework].[ImageFile] imageFile
where 1=1
    and imageFile.IsDeleted = 0