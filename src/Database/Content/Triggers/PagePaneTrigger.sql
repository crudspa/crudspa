create trigger [Content].[PagePaneTrigger] on [Content].[PagePane]
    for update
as

insert [Content].[PagePane] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PaneId
    ,PageId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PaneId
    ,deleted.PageId
from deleted