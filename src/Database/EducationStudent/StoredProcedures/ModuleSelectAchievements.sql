create proc [EducationStudent].[ModuleSelectAchievements] (
     @ModuleId uniqueidentifier
) as

select
    module.Id as Id
    ,module.Title as Title
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
from [Education].[Module-Active] module
    left join [Education].[Achievement-Active] generatesAchievement on module.GeneratesAchievementId = generatesAchievement.Id
    left join [Education].[Achievement-Active] requiresAchievement on module.RequiresAchievementId = requiresAchievement.Id
where module.Id = @ModuleId