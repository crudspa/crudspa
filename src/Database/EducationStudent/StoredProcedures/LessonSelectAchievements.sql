create proc [EducationStudent].[LessonSelectAchievements] (
     @ObjectiveId uniqueidentifier
) as

select
    lesson.Id as Id
    ,lesson.Title as Title
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
from [Education].[Lesson-Active] lesson
    inner join [Education].[Objective-Active] objective on objective.LessonId = lesson.Id
    left join [Education].[Achievement-Active] generatesAchievement on lesson.GeneratesAchievementId = generatesAchievement.Id
    left join [Education].[Achievement-Active] requiresAchievement on lesson.RequiresAchievementId = requiresAchievement.Id
where objective.Id = @ObjectiveId