create trigger [Framework].[DeviceJobTypeTrigger] on [Framework].[DeviceJobType]
    for update
as

insert [Framework].[DeviceJobType] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,DeviceId
    ,JobTypeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.DeviceId
    ,deleted.JobTypeId
from deleted