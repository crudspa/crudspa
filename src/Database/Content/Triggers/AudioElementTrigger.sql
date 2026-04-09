create trigger [Content].[AudioElementTrigger] on [Content].[AudioElement]
    for update
as

insert [Content].[AudioElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,FileId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.FileId
from deleted