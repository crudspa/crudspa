create proc [FrameworkCore].[ContactSelectNames] (
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
     contact.Id
    ,trim(concat(contact.FirstName, ' ', contact.LastName)) as Name
from [Framework].[Contact-Active] contact
where exists (
    select 1
    from [Content].[Membership-Active] membership
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    where portal.OwnerId = @organizationId
)
order by trim(concat(contact.FirstName, ' ', contact.LastName))