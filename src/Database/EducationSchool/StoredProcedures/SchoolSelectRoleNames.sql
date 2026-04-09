create proc [EducationSchool].[SchoolSelectRoleNames] (
     @SessionId uniqueidentifier
) as

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.ContactId = userTable.ContactId
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

where 1 = 1
    and school.Id = @schoolId
order by role.Name