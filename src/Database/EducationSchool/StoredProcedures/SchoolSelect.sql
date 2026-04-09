create proc [EducationSchool].[SchoolSelect] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     school.Id
    ,school.OrganizationId
from [Education].[School-Active] school
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
where 1 = 1
    and organization.Id = @organizationId