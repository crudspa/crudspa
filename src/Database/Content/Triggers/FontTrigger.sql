create trigger [Content].[FontTrigger] on [Content].[Font]
    for update
as

insert [Content].[Font] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Name
    ,ContentPortalId
    ,FileId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Name
    ,deleted.ContentPortalId
    ,deleted.FileId
from deleted