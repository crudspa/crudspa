create view [Education].[StudentAchievement-Active] as

select studentAchievement.Id as Id
    ,studentAchievement.StudentId as StudentId
    ,studentAchievement.AchievementId as AchievementId
    ,studentAchievement.RelatedEntityId as RelatedEntityId
    ,studentAchievement.Earned as Earned
from [Education].[StudentAchievement] studentAchievement
where 1=1
    and studentAchievement.IsDeleted = 0
    and studentAchievement.VersionOf = studentAchievement.Id