create view [Content].[ButtonElement-Active] as

select buttonElement.Id as Id
    ,buttonElement.ElementId as ElementId
    ,buttonElement.ButtonId as ButtonId
from [Content].[ButtonElement] buttonElement
where 1=1
    and buttonElement.IsDeleted = 0
    and buttonElement.VersionOf = buttonElement.Id