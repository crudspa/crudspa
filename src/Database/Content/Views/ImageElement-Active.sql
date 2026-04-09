create view [Content].[ImageElement-Active] as

select imageElement.Id as Id
    ,imageElement.ElementId as ElementId
    ,imageElement.FileId as FileId
    ,imageElement.HyperlinkUrl as HyperlinkUrl
from [Content].[ImageElement] imageElement
where 1=1
    and imageElement.IsDeleted = 0
    and imageElement.VersionOf = imageElement.Id