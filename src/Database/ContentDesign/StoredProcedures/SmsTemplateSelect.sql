create proc [ContentDesign].[SmsTemplateSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     smsTemplate.Id
    ,smsTemplate.PortalId
    ,smsTemplate.Title
    ,smsTemplate.Body
from [Content].[SmsTemplate-Active] smsTemplate
    inner join [Framework].[Portal-Active] portal on smsTemplate.PortalId = portal.Id
where smsTemplate.Id = @Id