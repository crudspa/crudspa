create trigger [Framework].[PortalSegmentTypeTrigger] on [Framework].[PortalSegmentType]
    for update
as

insert [Framework].[PortalSegmentType] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,TypeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.TypeId
from deleted