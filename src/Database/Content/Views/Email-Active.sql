create view [Content].[Email-Active] as

select email.Id as Id
    ,email.MembershipId as MembershipId
    ,email.FromName as FromName
    ,email.FromEmail as FromEmail
    ,email.TemplateId as TemplateId
    ,email.Send as Send
    ,email.Subject as Subject
    ,email.Body as Body
    ,email.Status as Status
    ,email.Processed as Processed
    ,email.BatchId as BatchId
from [Content].[Email] email
where 1=1
    and email.IsDeleted = 0
    and email.VersionOf = email.Id