create view [Content].[TextElement-Active] as

select textElement.Id as Id
    ,textElement.ElementId as ElementId
    ,textElement.Text as Text
from [Content].[TextElement] textElement
where 1=1
    and textElement.IsDeleted = 0
    and textElement.VersionOf = textElement.Id