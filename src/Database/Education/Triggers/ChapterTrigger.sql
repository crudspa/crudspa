create trigger [Education].[ChapterTrigger] on [Education].[Chapter]
    for update
as

insert [Education].[Chapter] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Title
    ,BookId
    ,BinderId
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
    ,deleted.BinderId
    ,deleted.Ordinal
from deleted