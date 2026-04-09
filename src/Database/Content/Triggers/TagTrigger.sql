create trigger [Content].[TagTrigger] on [Content].[Tag]
    for update
as

insert [Content].[Tag] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Title
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Title
from deleted