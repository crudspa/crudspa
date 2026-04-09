create trigger [Content].[ForumTrigger] on [Content].[Forum]
    for update
as

insert [Content].[Forum] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,StatusId
    ,Title
    ,Description
    ,ImageId
    ,Ordinal
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
    ,deleted.Description
    ,deleted.ImageId
    ,deleted.Ordinal
from deleted