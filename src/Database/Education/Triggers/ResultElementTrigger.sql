create trigger [Education].[ResultElementTrigger] on [Education].[ResultElement]
    for update
as

insert [Education].[ResultElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,ActivityElementId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.ActivityElementId
from deleted