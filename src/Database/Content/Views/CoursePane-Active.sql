create view [Content].[CoursePane-Active] as

select coursePane.Id as Id
    ,coursePane.PaneId as PaneId
    ,coursePane.IdSource as IdSource
    ,coursePane.CourseId as CourseId
from [Content].[CoursePane] coursePane
where 1=1
    and coursePane.IsDeleted = 0
    and coursePane.VersionOf = coursePane.Id