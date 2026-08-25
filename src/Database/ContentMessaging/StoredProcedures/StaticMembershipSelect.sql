create proc [ContentMessaging].[StaticMembershipSelect] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@PopulationId uniqueidentifier
    ,@OrganizationId uniqueidentifier
) as

set nocount on

if not exists (select 1 from [ContentMessaging].[SessionCanReadOrganization](@SessionId, @PortalId, @OrganizationId))
    throw 51000, 'Population access denied.', 1

declare @membershipId uniqueidentifier = (
    select top (1) membership.Id
    from [Content].[Membership-Active] membership
    where membership.PortalId = @PortalId
        and membership.PopulationId = @PopulationId
        and membership.OrganizationId is null
        and membership.ActivationScopeId is null
    order by membership.Id
)

select distinct
     contact.Id
    ,contact.FirstName
    ,contact.LastName
    ,contactEmail.Email
from [Content].[Member-Active] member
    inner join [Content].[Membership-Active] membership on membership.Id = member.MembershipId
    inner join [Framework].[Contact-Active] contact on contact.Id = member.ContactId
    outer apply (
        select top (1) contactEmail.Email
        from [Framework].[ContactEmail-Active] contactEmail
        where contactEmail.ContactId = contact.Id
        order by contactEmail.Ordinal
    ) contactEmail
where membership.Id = @membershipId
    and member.Status in (0, 1)
order by contact.LastName, contact.FirstName, contact.Id