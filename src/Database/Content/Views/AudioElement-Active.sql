create view [Content].[AudioElement-Active] as

select audioElement.Id as Id
    ,audioElement.ElementId as ElementId
    ,audioElement.FileId as FileId
from [Content].[AudioElement] audioElement
where 1=1
    and audioElement.IsDeleted = 0
    and audioElement.VersionOf = audioElement.Id