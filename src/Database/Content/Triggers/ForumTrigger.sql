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
    ,PermissionId
    ,Title
    ,Description
    ,ImageId
    ,AccessMode
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
    ,deleted.PermissionId
    ,deleted.Title
    ,deleted.Description
    ,deleted.ImageId
    ,deleted.AccessMode
    ,deleted.Ordinal
from deleted