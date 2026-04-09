create trigger [Content].[EmailTrigger] on [Content].[Email]
    for update
as

insert [Content].[Email] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MembershipId
    ,FromName
    ,FromEmail
    ,TemplateId
    ,Send
    ,Subject
    ,Body
    ,Status
    ,Processed
    ,BatchId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.MembershipId
    ,deleted.FromName
    ,deleted.FromEmail
    ,deleted.TemplateId
    ,deleted.Send
    ,deleted.Subject
    ,deleted.Body
    ,deleted.Status
    ,deleted.Processed
    ,deleted.BatchId
from deleted