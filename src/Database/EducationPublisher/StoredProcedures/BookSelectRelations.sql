create proc [EducationPublisher].[BookSelectRelations] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
) as

set nocount on

select distinct
     @BookId as RootId
    ,grade.Id as Id
    ,grade.Name as Name
    ,convert(bit, case when bookGrade.Id is null then 0 else 1 end) as Selected
    ,grade.Ordinal
from [Education].[Grade-Active] grade
    left join [Education].[BookGrade-Active] bookGrade on bookGrade.GradeId = grade.Id
        and bookGrade.BookId = @BookId
order by grade.Ordinal

select distinct
     @BookId as RootId
    ,schoolYear.Id as Id
    ,schoolYear.Name as Name
    ,convert(bit, case when bookSchoolYear.Id is null then 0 else 1 end) as Selected
from [Education].[SchoolYear-Active] schoolYear
    left join [Education].[BookSchoolYear-Active] bookSchoolYear on bookSchoolYear.SchoolYearId = schoolYear.Id
        and bookSchoolYear.BookId = @BookId
order by schoolYear.Name