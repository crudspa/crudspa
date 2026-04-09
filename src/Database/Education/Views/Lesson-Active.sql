create view [Education].[Lesson-Active] as

select lesson.Id as Id
    ,lesson.Title as Title
    ,lesson.StatusId as StatusId
    ,lesson.UnitId as UnitId
    ,lesson.ImageId as ImageId
    ,lesson.GuideImageId as GuideImageId
    ,lesson.GuideText as GuideText
    ,lesson.GuideAudioId as GuideAudioId
    ,lesson.RequiresAchievementId as RequiresAchievementId
    ,lesson.GeneratesAchievementId as GeneratesAchievementId
    ,lesson.RequireSequentialCompletion as RequireSequentialCompletion
    ,lesson.Treatment as Treatment
    ,lesson.Control as Control
    ,lesson.Ordinal as Ordinal
from [Education].[Lesson] lesson
where 1=1
    and lesson.IsDeleted = 0
    and lesson.VersionOf = lesson.Id