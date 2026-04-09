create view [Education].[Classroom-Active] as

select classroom.Id as Id
    ,classroom.OrganizationId as OrganizationId
    ,classroom.SchoolId as SchoolId
    ,classroom.GradeId as GradeId
    ,classroom.SchoolYearId as SchoolYearId
    ,classroom.TypeId as TypeId
    ,classroom.ImportNum as ImportNum
    ,classroom.SmallClassroom as SmallClassroom
from [Education].[Classroom] classroom
where 1=1
    and classroom.IsDeleted = 0
    and classroom.VersionOf = classroom.Id