create view [Framework].[ExportFile-Active] as

select exportFile.Id as Id
    ,exportFile.BlobId as BlobId
    ,exportFile.Name as Name
    ,exportFile.Format as Format
    ,exportFile.Description as Description
    ,exportFile.Published as Published
from [Framework].[ExportFile] exportFile
where 1=1
    and exportFile.IsDeleted = 0