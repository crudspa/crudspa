create trigger [Education].[StudentNotebookTrigger] on [Education].[StudentNotebook]
    for update
as

insert [Education].[StudentNotebook] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,StudentId
    ,NotebookId
    ,UnitId
    ,BookId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.StudentId
    ,deleted.NotebookId
    ,deleted.UnitId
    ,deleted.BookId
from deleted