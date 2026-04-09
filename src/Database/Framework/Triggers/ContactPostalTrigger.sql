create trigger [Framework].[ContactPostalTrigger] on [Framework].[ContactPostal]
    for update
as

insert [Framework].[ContactPostal] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,PostalId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.PostalId
    ,deleted.Ordinal
from deleted