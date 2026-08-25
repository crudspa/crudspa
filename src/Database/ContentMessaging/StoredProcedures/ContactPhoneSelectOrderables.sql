create proc [ContentMessaging].[ContactPhoneSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     contactPhone.Id
    ,contactPhone.Phone as Name
    ,contactPhone.Ordinal
from [Framework].[ContactPhone-Active] contactPhone
where exists (
    select 1
    from [Content].[Member-Active] member
        inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where member.ContactId = contactPhone.ContactId
)
order by contactPhone.Ordinal