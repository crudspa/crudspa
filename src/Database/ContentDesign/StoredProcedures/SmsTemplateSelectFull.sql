create proc [ContentDesign].[SmsTemplateSelectFull]  (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     smsTemplate.Id
    ,smsTemplate.PortalId
    ,smsTemplate.Title
    ,smsTemplate.Body
from [Content].[SmsTemplate-Active] smsTemplate
    inner join [Framework].[Portal-Active] portal on smsTemplate.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where smsTemplate.PortalId = @PortalId
    and organization.Id = @organizationId
order by smsTemplate.Title