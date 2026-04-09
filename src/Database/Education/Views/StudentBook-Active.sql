create view [Education].[StudentBook-Active] as

select studentBook.Id as Id
    ,studentBook.StudentId as StudentId
    ,studentBook.BookId as BookId
    ,studentBook.SchoolYearId as SchoolYearId
from [Education].[StudentBook] studentBook
where 1=1
    and studentBook.IsDeleted = 0
    and studentBook.VersionOf = studentBook.Id