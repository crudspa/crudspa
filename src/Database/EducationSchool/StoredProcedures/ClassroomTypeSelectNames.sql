create proc [EducationSchool].[ClassroomTypeSelectNames] as

select
    classroomType.Id
    ,classroomType.[Name] as Name
    ,classroomType.Ordinal as Ordinal
from [Education].[ClassroomType-Active] as classroomType
order by classroomType.Ordinal