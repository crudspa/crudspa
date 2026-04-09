create proc [EducationStudent].[UnitSelectAchievements] (
     @ObjectiveId uniqueidentifier
) as

select
     unit.Id as Id
    ,unit.Title as Title
    ,generatesAchievement.Id as GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,generatesAchievement.RarityId as GeneratesAchievementRarityId
    ,generatesAchievement.TrophyImageId as GeneratesAchievementTrophyImageId
    ,generatesAchievement.VisibleToStudents as GeneratesAchievementVisibleToStudents
    ,requiresAchievement.Id as RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,requiresAchievement.RarityId as RequiresAchievementRarityId
    ,requiresAchievement.TrophyImageId as RequiresAchievementTrophyImageId
    ,requiresAchievement.VisibleToStudents as RequiresAchievementVisibleToStudents
from [Education].[Unit-Active] unit
    inner join [Education].[Lesson-Active] lesson on lesson.UnitId = unit.Id
    inner join [Education].[Objective-Active] objective on objective.LessonId = lesson.Id
    left join [Education].[Achievement-Active] generatesAchievement on unit.GeneratesAchievementId = generatesAchievement.Id
    left join [Education].[Achievement-Active] requiresAchievement on unit.RequiresAchievementId = requiresAchievement.Id
where objective.Id = @ObjectiveId