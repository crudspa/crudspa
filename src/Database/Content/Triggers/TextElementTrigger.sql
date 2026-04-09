create trigger [Content].[TextElementTrigger] on [Content].[TextElement]
    for update
as

insert [Content].[TextElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,Text
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.Text
from deleted