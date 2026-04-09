create view [Content].[MultimediaElement-Active] as

select multimediaElement.Id as Id
    ,multimediaElement.ElementId as ElementId
    ,multimediaElement.ContainerId as ContainerId
from [Content].[MultimediaElement] multimediaElement
where 1=1
    and multimediaElement.IsDeleted = 0
    and multimediaElement.VersionOf = multimediaElement.Id