create trigger [Education].[ObjectiveTrigger] on [Education].[Objective]
    for update
as

insert [Education].[Objective] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Title
    ,StatusId
    ,LessonId
    ,TrophyImageId
    ,BinderId
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
    ,deleted.Title
    ,deleted.StatusId
    ,deleted.LessonId
    ,deleted.TrophyImageId
    ,deleted.BinderId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
    ,deleted.Ordinal
from deleted