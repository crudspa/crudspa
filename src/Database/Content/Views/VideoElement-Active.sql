create view [Content].[VideoElement-Active] as

select videoElement.Id as Id
    ,videoElement.ElementId as ElementId
    ,videoElement.FileId as FileId
    ,videoElement.AutoPlay as AutoPlay
    ,videoElement.PosterId as PosterId
from [Content].[VideoElement] videoElement
where 1=1
    and videoElement.IsDeleted = 0
    and videoElement.VersionOf = videoElement.Id