create proc [EducationSchool].[DistrictSelectBySession] (
     @SessionId uniqueidentifier
) as

select district.Id as Id
    ,district.StudentIdNumberLabel as StudentIdNumberLabel
    ,district.AssessmentExplainer as AssessmentExplainer
    ,organization.Name as OrganizationName
from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.UserId = userTable.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
where session.Id = @SessionId