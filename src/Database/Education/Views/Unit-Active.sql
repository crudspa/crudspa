create view [Education].[Unit-Active] as

select unit.Id as Id
    ,unit.OwnerId as OwnerId
    ,unit.Title as Title
    ,unit.StatusId as StatusId
    ,unit.GradeId as GradeId
    ,unit.ParentId as ParentId
    ,unit.ImageId as ImageId
    ,unit.IntroAudioId as IntroAudioId
    ,unit.SongAudioId as SongAudioId
    ,unit.GuideText as GuideText
    ,unit.GuideAudioId as GuideAudioId
    ,unit.RequiresAchievementId as RequiresAchievementId
    ,unit.GeneratesAchievementId as GeneratesAchievementId
    ,unit.Treatment as Treatment
    ,unit.Control as Control
    ,unit.Ordinal as Ordinal
from [Education].[Unit] unit
where 1=1
    and unit.IsDeleted = 0
    and unit.VersionOf = unit.Id