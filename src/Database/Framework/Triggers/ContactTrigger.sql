create trigger [Framework].[ContactTrigger] on [Framework].[Contact]
    for update
as

insert [Framework].[Contact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,FirstName
    ,LastName
    ,TimeZoneId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.FirstName
    ,deleted.LastName
    ,deleted.TimeZoneId
from deleted