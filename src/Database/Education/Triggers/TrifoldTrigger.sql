create trigger [Education].[TrifoldTrigger] on [Education].[Trifold]
    for update
as

insert [Education].[Trifold] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Title
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
    ,deleted.BookId
    ,deleted.StatusId
    ,deleted.BinderId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
    ,deleted.Ordinal
from deleted