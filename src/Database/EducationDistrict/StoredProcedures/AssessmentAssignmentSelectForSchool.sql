create proc [EducationDistrict].[AssessmentAssignmentSelectForSchool] (
     @SessionId uniqueidentifier
    ,@AssessmentId uniqueidentifier
    ,@SchoolId uniqueidentifier
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

select distinct
    assessmentAssignment.Id as Id
    ,assessmentAssignment.AssessmentId as AssessmentId
    ,assessmentAssignment.StudentId as StudentId
from [Education].[AssessmentAssignment-Active] assessmentAssignment
    inner join [Education].[Assessment-Active] assessment on assessmentAssignment.AssessmentId = assessment.Id
    inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
where assessmentAssignment.AssessmentId = @AssessmentId
    and assessmentAssignment.Terminated is null
    and assessmentAssignment.Completed is null
    and student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
    and school.Id = @SchoolId
    and school.DistrictId = @districtId