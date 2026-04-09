create trigger [Content].[SectionTrigger] on [Content].[Section]
    for update
as

insert [Content].[Section] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PageId
    ,TypeId
    ,BoxId
    ,ContainerId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PageId
    ,deleted.TypeId
    ,deleted.BoxId
    ,deleted.ContainerId
    ,deleted.Ordinal
from deleted