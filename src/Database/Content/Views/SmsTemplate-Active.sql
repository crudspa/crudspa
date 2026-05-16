create view [Content].[SmsTemplate-Active] as

select smsTemplate.Id as Id
    ,smsTemplate.MembershipId as MembershipId
    ,smsTemplate.Title as Title
    ,smsTemplate.Body as Body
from [Content].[SmsTemplate] smsTemplate
where 1=1
    and smsTemplate.IsDeleted = 0
    and smsTemplate.VersionOf = smsTemplate.Id