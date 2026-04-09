create proc [FrameworkCore].[FontFileSelect] (
     @Id uniqueidentifier
) as

select
    fontFile.Id
    ,fontFile.BlobId
    ,fontFile.Name
    ,fontFile.Format
    ,fontFile.Description
from [Framework].[FontFile-Active] fontFile
where fontFile.Id = @Id