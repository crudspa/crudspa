create trigger [Content].[TrackTrigger] on [Content].[Track]
    for update
as

insert [Content].[Track] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,Title
    ,Description
    ,StatusId
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,RequireSequentialCompletion
    ,Ordinal
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
    ,deleted.StatusId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
    ,deleted.RequireSequentialCompletion
    ,deleted.Ordinal
from deleted