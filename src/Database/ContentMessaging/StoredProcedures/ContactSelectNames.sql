create proc [ContentMessaging].[ContactSelectNames] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     contact.Id
    ,contact.FirstName as Name
from [Framework].[Contact-Active] contact
where exists (
    select 1
    from [Content].[Member-Active] member
        inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where member.ContactId = contact.Id
)
order by contact.FirstName