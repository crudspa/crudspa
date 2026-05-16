create trigger [Content].[SmsTrigger] on [Content].[Sms]
    for update
as

insert [Content].[Sms] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MembershipId
    ,SmsChannelKey
    ,TemplateId
    ,Send
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
    ,deleted.SmsChannelKey
    ,deleted.TemplateId
    ,deleted.Send
    ,deleted.Body
    ,deleted.Status
    ,deleted.Processed
    ,deleted.BatchId
from deleted