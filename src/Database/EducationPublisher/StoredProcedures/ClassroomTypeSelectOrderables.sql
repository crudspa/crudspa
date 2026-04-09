create proc [EducationPublisher].[ClassroomTypeSelectOrderables] as

set nocount on
select
     classroomType.Id
    ,classroomType.Name as Name
    ,classroomType.Ordinal
from [Education].[ClassroomType-Active] classroomType
order by classroomType.Ordinal