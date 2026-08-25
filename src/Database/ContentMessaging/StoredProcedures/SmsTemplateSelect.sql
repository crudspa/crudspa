create proc [ContentMessaging].[SmsTemplateSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     smsTemplate.Id
    ,smsTemplate.MembershipId
    ,smsTemplate.PortalId
    ,smsTemplate.OrganizationId
    ,smsTemplate.Title
    ,smsTemplate.Body
from [Content].[SmsTemplate-Active] smsTemplate
    inner join [Framework].[Portal-Active] portal on smsTemplate.PortalId = portal.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, smsTemplate.PortalId, smsTemplate.OrganizationId)
where smsTemplate.Id = @Id