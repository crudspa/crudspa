create proc [EducationPublisher].[PublisherSelect] (
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
     publisher.Id
    ,publisher.OrganizationId
from [Education].[Publisher-Active] publisher
    inner join [Framework].[Organization-Active] organization on publisher.OrganizationId = organization.Id
where 1 = 1
    and organization.Id = @organizationId