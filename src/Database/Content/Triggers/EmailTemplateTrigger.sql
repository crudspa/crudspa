create trigger [Content].[EmailTemplateTrigger] on [Content].[EmailTemplate]
    for update
as

insert [Content].[EmailTemplate] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MembershipId
    ,Title
    ,Subject
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
    ,deleted.Subject
    ,deleted.Body
from deleted