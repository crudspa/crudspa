create view [Education].[ClassroomType-Active] as

select classroomType.Id as Id
    ,classroomType.Name as Name
    ,classroomType.Ordinal as Ordinal
from [Education].[ClassroomType] classroomType
where 1=1
    and classroomType.IsDeleted = 0