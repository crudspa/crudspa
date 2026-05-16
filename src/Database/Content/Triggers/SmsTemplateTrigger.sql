create trigger [Content].[SmsTemplateTrigger] on [Content].[SmsTemplate]
    for update
as

insert [Content].[SmsTemplate] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MembershipId
    ,Title
    ,Body
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.MembershipId
    ,deleted.Title
    ,deleted.Body
from deleted