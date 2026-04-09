create trigger [Education].[ModuleTrigger] on [Education].[Module]
    for update
as

insert [Education].[Module] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Title
    ,IconId
    ,IconName
    ,BookId
    ,StatusId
    ,BinderId
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Title
    ,deleted.IconId
    ,deleted.IconName
    ,deleted.BookId
    ,deleted.StatusId
    ,deleted.BinderId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
    ,deleted.Ordinal
from deleted