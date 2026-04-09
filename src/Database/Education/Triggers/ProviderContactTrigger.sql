create trigger [Education].[ProviderContactTrigger] on [Education].[ProviderContact]
    for update
as

insert [Education].[ProviderContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ProviderId
    ,ContactId
    ,UserId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ProviderId
    ,deleted.ContactId
    ,deleted.UserId
from deleted