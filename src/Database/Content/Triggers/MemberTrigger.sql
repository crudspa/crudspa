create trigger [Content].[MemberTrigger] on [Content].[Member]
    for update
as

insert [Content].[Member] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MembershipId
    ,ContactId
    ,Status
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.MembershipId
    ,deleted.ContactId
    ,deleted.Status
from deleted