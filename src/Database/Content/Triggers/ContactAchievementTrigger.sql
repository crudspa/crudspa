create trigger [Content].[ContactAchievementTrigger] on [Content].[ContactAchievement]
    for update
as

insert [Content].[ContactAchievement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,AchievementId
    ,RelatedEntityId
    ,Earned
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.AchievementId
    ,deleted.RelatedEntityId
    ,deleted.Earned
from deleted