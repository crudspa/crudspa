create trigger [Content].[ButtonElementTrigger] on [Content].[ButtonElement]
    for update
as

insert [Content].[ButtonElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,ButtonId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.ButtonId
from deleted