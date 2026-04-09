create proc [EducationDistrict].[StudentSelectForDistrict] (
     @SessionId uniqueidentifier
    ,@AssessmentId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

declare @gradeId uniqueidentifier = (select GradeId from [Education].[Assessment-Active] where Id = @AssessmentId)

declare @districtId uniqueidentifier = (
    select top 1 districtContact.DistrictId
    from [Education].[DistrictContact-Active] districtContact
        inner join [Framework].[Session-Active] session on session.UserId = districtContact.UserId
    where session.Id = @SessionId
)

select distinct
    student.Id
from [Education].[Student-Active] student
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
where student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
    and student.IsTestAccount = 0
    and student.GradeId = @gradeId
    and district.Id = @districtId