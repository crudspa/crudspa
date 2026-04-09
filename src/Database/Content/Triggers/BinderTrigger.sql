create trigger [Content].[BinderTrigger] on [Content].[Binder]
    for update
as

insert [Content].[Binder] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,TypeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.TypeId
from deleted