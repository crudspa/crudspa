create trigger [Content].[VideoElementTrigger] on [Content].[VideoElement]
    for update
as

insert [Content].[VideoElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,FileId
    ,AutoPlay
    ,PosterId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.FileId
    ,deleted.AutoPlay
    ,deleted.PosterId
from deleted