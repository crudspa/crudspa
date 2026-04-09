create trigger [Framework].[ContactPhoneTrigger] on [Framework].[ContactPhone]
    for update
as

insert [Framework].[ContactPhone] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,Phone
    ,Extension
    ,SupportsSms
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.Phone
    ,deleted.Extension
    ,deleted.SupportsSms
    ,deleted.Ordinal
from deleted