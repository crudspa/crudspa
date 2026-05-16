create proc [ContentDesign].[SmsTemplateSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     smsTemplate.Id
    ,smsTemplate.MembershipId
    ,smsTemplate.Title
    ,smsTemplate.Body
from [Content].[SmsTemplate-Active] smsTemplate
    inner join [Content].[Membership-Active] membership on smsTemplate.MembershipId = membership.Id
where smsTemplate.Id = @Id