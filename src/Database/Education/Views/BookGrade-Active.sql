create view [Education].[BookGrade-Active] as

select bookGrade.Id as Id
    ,bookGrade.BookId as BookId
    ,bookGrade.GradeId as GradeId
from [Education].[BookGrade] bookGrade
where 1=1
    and bookGrade.IsDeleted = 0
    and bookGrade.VersionOf = bookGrade.Id