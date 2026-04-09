create trigger [Education].[ActivityElementTrigger] on [Education].[ActivityElement]
    for update
as

insert [Education].[ActivityElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,ActivityId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.ActivityId
from deleted