create view [Education].[ResultElement-Active] as

select resultElement.Id as Id
    ,resultElement.ElementId as ElementId
    ,resultElement.ActivityElementId as ActivityElementId
from [Education].[ResultElement] resultElement
where 1=1
    and resultElement.IsDeleted = 0
    and resultElement.VersionOf = resultElement.Id