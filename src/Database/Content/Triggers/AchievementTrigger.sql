create trigger [Content].[AchievementTrigger] on [Content].[Achievement]
    for update
as

insert [Content].[Achievement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,Title
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
    ,deleted.Title
    ,deleted.Description
    ,deleted.ImageId
from deleted