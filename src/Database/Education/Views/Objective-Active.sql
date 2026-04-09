create view [Education].[Objective-Active] as

select objective.Id as Id
    ,objective.Title as Title
    ,objective.StatusId as StatusId
    ,objective.LessonId as LessonId
    ,objective.TrophyImageId as TrophyImageId
    ,objective.BinderId as BinderId
    ,objective.RequiresAchievementId as RequiresAchievementId
    ,objective.GeneratesAchievementId as GeneratesAchievementId
    ,objective.Ordinal as Ordinal
from [Education].[Objective] objective
where 1=1
    and objective.IsDeleted = 0
    and objective.VersionOf = objective.Id