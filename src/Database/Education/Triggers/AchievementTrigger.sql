create trigger [Education].[AchievementTrigger] on [Education].[Achievement]
    for update
as

insert [Education].[Achievement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OwnerId
    ,Title
    ,RarityId
    ,TrophyImageId
    ,VisibleToStudents
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OwnerId
    ,deleted.Title
    ,deleted.RarityId
    ,deleted.TrophyImageId
    ,deleted.VisibleToStudents
from deleted