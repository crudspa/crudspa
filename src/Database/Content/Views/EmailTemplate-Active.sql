create view [Content].[EmailTemplate-Active] as

select emailTemplate.Id as Id
    ,emailTemplate.MembershipId as MembershipId
    ,emailTemplate.Title as Title
    ,emailTemplate.Subject as Subject
    ,emailTemplate.Body as Body
from [Content].[EmailTemplate] emailTemplate
where 1=1
    and emailTemplate.IsDeleted = 0
    and emailTemplate.VersionOf = emailTemplate.Id