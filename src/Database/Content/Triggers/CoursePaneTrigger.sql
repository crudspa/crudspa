create trigger [Content].[CoursePaneTrigger] on [Content].[CoursePane]
    for update
as

insert [Content].[CoursePane] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PaneId
    ,IdSource
    ,CourseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PaneId
    ,deleted.IdSource
    ,deleted.CourseId
from deleted