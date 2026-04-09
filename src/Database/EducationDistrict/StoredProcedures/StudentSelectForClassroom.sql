create proc [EducationDistrict].[StudentSelectForClassroom] (
     @SessionId uniqueidentifier
    ,@ClassroomId uniqueidentifier
    ,@AssessmentId uniqueidentifier
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

declare @gradeId uniqueidentifier = (select GradeId from [Education].[Assessment-Active] where Id = @AssessmentId)

select distinct
    student.Id
from [Education].[Student-Active] student
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
    inner join [Education].[Classroom-Active] classroom on classroom.SchoolId = school.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
where student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
    and student.IsTestAccount = 0
    and classroom.SchoolYearId = @schoolYearId
    and classroom.Id = @ClassroomId
    and district.Id = @districtId
    and student.Id in (
        select StudentId
        from [Education].[ClassroomStudent-Active]
        where ClassroomId = @ClassroomId
            and student.GradeId = @gradeId
    )