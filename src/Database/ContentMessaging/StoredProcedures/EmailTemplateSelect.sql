create proc [ContentMessaging].[EmailTemplateSelect] (
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
     emailTemplate.Id
    ,emailTemplate.MembershipId
    ,emailTemplate.PortalId
    ,emailTemplate.OrganizationId
    ,emailTemplate.Title
    ,emailTemplate.Subject
    ,emailTemplate.Body
from [Content].[EmailTemplate-Active] emailTemplate
    inner join [Framework].[Portal-Active] portal on emailTemplate.PortalId = portal.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, emailTemplate.PortalId, emailTemplate.OrganizationId)
where emailTemplate.Id = @Id