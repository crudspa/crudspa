create trigger [Framework].[OrganizationTrigger] on [Framework].[Organization]
    for update
as

insert [Framework].[Organization] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Name
    ,TimeZoneId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Name
    ,deleted.TimeZoneId
from deleted