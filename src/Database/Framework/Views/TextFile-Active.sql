create view [Framework].[TextFile-Active] as

select textFile.Id as Id
    ,textFile.BlobId as BlobId
from [Framework].[TextFile] textFile
where 1=1
    and textFile.IsDeleted = 0