create view [Content].[Sms-Active] as

select sms.Id as Id
    ,sms.MembershipId as MembershipId
    ,sms.SmsChannelKey as SmsChannelKey
    ,sms.TemplateId as TemplateId
    ,sms.Send as Send
    ,sms.Body as Body
    ,sms.Status as Status
    ,sms.Processed as Processed
    ,sms.BatchId as BatchId
from [Content].[Sms] sms
where 1=1
    and sms.IsDeleted = 0
    and sms.VersionOf = sms.Id