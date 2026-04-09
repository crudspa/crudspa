create proc [EducationDistrict].[SchoolSelectRoleNames] (
     @SessionId uniqueidentifier
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

set nocount on

select
     role.Id
    ,role.Name
from [Framework].[Role-Active] role
    inner join [Framework].[Organization-Active] organization on role.OrganizationId = organization.Id
    inner join [Education].[School-Active] school on school.OrganizationId = organization.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
where 1 = 1
    and school.Id = @SchoolId
    and district.Id = @districtId
order by role.Name