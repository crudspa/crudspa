create trigger [Framework].[ContactEmailTrigger] on [Framework].[ContactEmail]
    for update
as

insert [Framework].[ContactEmail] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,Email
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.Email
    ,deleted.Ordinal
from deleted