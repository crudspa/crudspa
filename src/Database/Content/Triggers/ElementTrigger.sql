create trigger [Content].[ElementTrigger] on [Content].[Element]
    for update
as

insert [Content].[Element] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SectionId
    ,TypeId
    ,RequireInteraction
    ,BoxId
    ,ItemId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SectionId
    ,deleted.TypeId
    ,deleted.RequireInteraction
    ,deleted.BoxId
    ,deleted.ItemId
    ,deleted.Ordinal
from deleted