create proc [ContentDesign].[EmailTemplateSelectFull]  (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     emailTemplate.Id
    ,emailTemplate.MembershipId
    ,emailTemplate.Title
    ,emailTemplate.Subject
    ,emailTemplate.Body
from [Content].[EmailTemplate-Active] emailTemplate
    inner join [Content].[Membership-Active] membership on emailTemplate.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where emailTemplate.MembershipId = @MembershipId
    and organization.Id = @organizationId
order by emailTemplate.Title