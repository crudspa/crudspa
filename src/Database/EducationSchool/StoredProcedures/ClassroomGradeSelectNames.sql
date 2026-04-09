create proc [EducationSchool].[ClassroomGradeSelectNames] as

select
    grade.Id
    ,grade.[Name] as Name
    ,grade.Ordinal as Ordinal
from [Education].[Grade-Active] as grade
order by grade.Ordinal