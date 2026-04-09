create proc [EducationSchool].[StudentSelectableClassrooms] (
     @SessionId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@SchoolYearId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

select
     @StudentId as RootId
    ,classroom.Id as Id
    ,organization.Name as Name
    ,convert(bit, case when classroomStudent.Id is null then 0 else 1 end) as Selected
from [Education].[Classroom-Active] classroom
    inner join [Framework].[Organization-Active] organization on classroom.OrganizationId = [Organization].Id
    inner join [Education].[Student-Active] student on student.Id = @StudentId
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
        and classroom.SchoolId = family.SchoolId
    left join [Education].[ClassroomStudent-Active] classroomStudent on classroomStudent.ClassroomId = classroom.Id
        and classroomStudent.StudentId = @StudentId
where classroom.SchoolYearId = @SchoolYearId
    and classroom.SchoolId in (select Id from [EducationSchool].[MySchools] (@SessionId))
order by organization.Name