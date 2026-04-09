create proc [EducationCommon].[GradeSelectOrderables] as

set nocount on

select
     grade.Id
    ,grade.Name as Name
    ,grade.Ordinal
from [Education].[Grade-Active] grade
order by grade.Ordinal