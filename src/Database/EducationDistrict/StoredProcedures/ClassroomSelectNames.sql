create proc [EducationDistrict].[ClassroomSelectNames] (
     @SessionId uniqueidentifier
    ,@SchoolId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

declare @districtId uniqueidentifier = (
    select top 1 districtContact.DistrictId
    from [Education].[DistrictContact-Active] districtContact
        inner join [Framework].[Session-Active] session on session.UserId = districtContact.UserId
    where session.Id = @SessionId
)

select distinct
     classroom.Id as Id
    ,organization.Name as Name
from [Education].[Classroom-Active] classroom
    inner join [Framework].[Organization-Active] organization on classroom.OrganizationId = organization.Id
    inner join [Education].[School-Active] school on classroom.SchoolId = school.Id
where school.DistrictId = @districtId
    and classroom.SchoolId = @SchoolId
    and classroom.SchoolYearId = @schoolYearId
order by organization.Name