create view [Content].[ContactAchievement-Active] as

select contactAchievement.Id as Id
    ,contactAchievement.ContactId as ContactId
    ,contactAchievement.AchievementId as AchievementId
    ,contactAchievement.RelatedEntityId as RelatedEntityId
    ,contactAchievement.Earned as Earned
from [Content].[ContactAchievement] contactAchievement
where 1=1
    and contactAchievement.IsDeleted = 0
    and contactAchievement.VersionOf = contactAchievement.Id