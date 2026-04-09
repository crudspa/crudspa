create trigger [Framework].[LicenseTrigger] on [Framework].[License]
    for update
as

insert [Framework].[License] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OwnerId
    ,Name
    ,Description
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OwnerId
    ,deleted.Name
    ,deleted.Description
from deleted