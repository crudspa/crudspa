create trigger [Content].[NoteElementTrigger] on [Content].[NoteElement]
    for update
as

insert [Content].[NoteElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,Instructions
    ,ImageFileId
    ,RequireText
    ,RequireImageSelection
    ,OpenOnly
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.Instructions
    ,deleted.ImageFileId
    ,deleted.RequireText
    ,deleted.RequireImageSelection
    ,deleted.OpenOnly
from deleted