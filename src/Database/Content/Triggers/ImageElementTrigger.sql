create trigger [Content].[ImageElementTrigger] on [Content].[ImageElement]
    for update
as

insert [Content].[ImageElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,FileId
    ,HyperlinkUrl
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.FileId
    ,deleted.HyperlinkUrl
from deleted