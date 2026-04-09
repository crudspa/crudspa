create trigger [Education].[LessonTrigger] on [Education].[Lesson]
    for update
as

insert [Education].[Lesson] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Title
    ,StatusId
    ,UnitId
    ,ImageId
    ,GuideImageId
    ,GuideText
    ,GuideAudioId
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,RequireSequentialCompletion
    ,Treatment
    ,Control
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
    ,deleted.UnitId
    ,deleted.ImageId
    ,deleted.GuideImageId
    ,deleted.GuideText
    ,deleted.GuideAudioId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
    ,deleted.RequireSequentialCompletion
    ,deleted.Treatment
    ,deleted.Control
    ,deleted.Ordinal
from deleted