create proc [ContentMessaging].[EmailTemplateSelectFull] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
) as

set nocount on

select
     emailTemplate.Id
    ,emailTemplate.MembershipId
    ,emailTemplate.Title
    ,emailTemplate.Subject
    ,emailTemplate.Body
from [Content].[EmailTemplate-Active] emailTemplate
    inner join [Content].[Membership-Active] membership on membership.Id = @MembershipId
    cross apply [ContentMessaging].[SessionCanReadOrganization](
        @SessionId, membership.PortalId, membership.OrganizationId)
where emailTemplate.PortalId = membership.PortalId
    and (emailTemplate.OrganizationId is null or emailTemplate.OrganizationId = membership.OrganizationId)
    and (emailTemplate.MembershipId is null or emailTemplate.MembershipId = membership.Id)
order by emailTemplate.Title