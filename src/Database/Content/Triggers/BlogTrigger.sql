create trigger [Content].[BlogTrigger] on [Content].[Blog]
    for update
as

insert [Content].[Blog] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,StatusId
    ,Title
    ,Author
    ,Description
    ,ImageId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.StatusId
    ,deleted.Title
    ,deleted.Author
    ,deleted.Description
    ,deleted.ImageId
from deleted