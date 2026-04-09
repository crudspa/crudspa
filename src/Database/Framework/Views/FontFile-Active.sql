create view [Framework].[FontFile-Active] as

select fontFile.Id as Id
    ,fontFile.BlobId as BlobId
    ,fontFile.Name as Name
    ,fontFile.Format as Format
    ,fontFile.Description as Description
from [Framework].[FontFile] fontFile
where 1=1
    and fontFile.IsDeleted = 0