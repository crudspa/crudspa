create trigger [Education].[GameSectionTrigger] on [Education].[GameSection]
    for update
as

insert [Education].[GameSection] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,GameId
    ,Title
    ,StatusId
    ,TypeId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.GameId
    ,deleted.Title
    ,deleted.StatusId
    ,deleted.TypeId
    ,deleted.Ordinal
from deleted