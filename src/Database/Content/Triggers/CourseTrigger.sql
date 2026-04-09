create trigger [Content].[CourseTrigger] on [Content].[Course]
    for update
as

insert [Content].[Course] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,TrackId
    ,BinderId
    ,Title
    ,Description
    ,StatusId
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.TrackId
    ,deleted.BinderId
    ,deleted.Title
    ,deleted.Description
    ,deleted.StatusId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
    ,deleted.Ordinal
from deleted