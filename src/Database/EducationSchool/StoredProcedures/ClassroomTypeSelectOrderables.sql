create proc [EducationSchool].[ClassroomTypeSelectOrderables] as

select
    classroomType.Id
    ,classroomType.Name
    ,classroomType.Ordinal
from [Education].[ClassroomType-Active] classroomType
order by classroomType.Ordinal