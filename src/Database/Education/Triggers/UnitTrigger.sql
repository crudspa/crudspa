create trigger [Education].[UnitTrigger] on [Education].[Unit]
    for update
as

insert [Education].[Unit] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OwnerId
    ,Title
    ,StatusId
    ,GradeId
    ,ParentId
    ,ImageId
    ,IntroAudioId
    ,SongAudioId
    ,GuideText
    ,GuideAudioId
    ,RequiresAchievementId
    ,GeneratesAchievementId
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
    ,deleted.OwnerId
    ,deleted.Title
    ,deleted.StatusId
    ,deleted.GradeId
    ,deleted.ParentId
    ,deleted.ImageId
    ,deleted.IntroAudioId
    ,deleted.SongAudioId
    ,deleted.GuideText
    ,deleted.GuideAudioId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
    ,deleted.Treatment
    ,deleted.Control
    ,deleted.Ordinal
from deleted