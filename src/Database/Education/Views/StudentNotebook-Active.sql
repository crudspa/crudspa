create view [Education].[StudentNotebook-Active] as

select studentNotebook.Id as Id
    ,studentNotebook.StudentId as StudentId
    ,studentNotebook.NotebookId as NotebookId
    ,studentNotebook.UnitId as UnitId
    ,studentNotebook.BookId as BookId
from [Education].[StudentNotebook] studentNotebook
where 1=1
    and studentNotebook.IsDeleted = 0
    and studentNotebook.VersionOf = studentNotebook.Id