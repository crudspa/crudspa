create proc [ContentMessaging].[MemberSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     member.Id
    ,member.MembershipId
    ,member.Status
    ,contact.Id as ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
from [Content].[Member-Active] member
    inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
    inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
where member.Id = @Id