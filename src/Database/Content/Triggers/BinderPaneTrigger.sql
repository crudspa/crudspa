create trigger [Content].[BinderPaneTrigger] on [Content].[BinderPane]
    for update
as

insert [Content].[BinderPane] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PaneId
    ,BinderId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PaneId
    ,deleted.BinderId
from deleted