create proc [EducationStudent].[ObjectiveSelectAchievements] (
     @ObjectiveId uniqueidentifier
) as

select
    objective.Id
    ,objective.Title
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
from [Education].[Objective-Active] objective
    left join [Education].[Achievement-Active] generatesAchievement on objective.GeneratesAchievementId = generatesAchievement.Id
    left join [Education].[Achievement-Active] requiresAchievement on objective.RequiresAchievementId = requiresAchievement.Id
where objective.Id = @ObjectiveId