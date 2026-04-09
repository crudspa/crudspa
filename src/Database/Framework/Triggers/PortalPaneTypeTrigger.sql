create trigger [Framework].[PortalPaneTypeTrigger] on [Framework].[PortalPaneType]
    for update
as

insert [Framework].[PortalPaneType] (
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