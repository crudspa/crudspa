create proc [EducationDistrict].[SchoolSelectNames] (
     @SessionId uniqueidentifier
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     school.Id
    ,organization.Name
from [Education].[School-Active] school
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
where district.Id = @districtId
order by organization.Name