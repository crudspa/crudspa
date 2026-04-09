create trigger [Education].[StudentAchievementTrigger] on [Education].[StudentAchievement]
    for update
as

insert [Education].[StudentAchievement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,StudentId
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
    ,deleted.StudentId
    ,deleted.AchievementId
    ,deleted.RelatedEntityId
    ,deleted.Earned
from deleted