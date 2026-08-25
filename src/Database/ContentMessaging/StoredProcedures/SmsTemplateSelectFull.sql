create proc [ContentMessaging].[SmsTemplateSelectFull] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

set nocount on

declare @organizationId uniqueidentifier
declare @ownerId uniqueidentifier

select
     @organizationId = userTable.OrganizationId
    ,@ownerId = portal.OwnerId
from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
    inner join [Framework].[Portal-Active] portal on portal.Id = @PortalId
where session.Id = @SessionId
    and (userTable.OrganizationId = portal.OwnerId or userTable.PortalId = portal.Id)

select
     smsTemplate.Id
    ,smsTemplate.PortalId
    ,smsTemplate.Title
    ,smsTemplate.Body
from [Content].[SmsTemplate-Active] smsTemplate
where @organizationId is not null
    and smsTemplate.PortalId = @PortalId
    and (@organizationId = @ownerId
        or smsTemplate.OrganizationId is null
        or smsTemplate.OrganizationId = @organizationId)
order by smsTemplate.Title