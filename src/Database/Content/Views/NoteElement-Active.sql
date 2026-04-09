create view [Content].[NoteElement-Active] as

select noteElement.Id as Id
    ,noteElement.ElementId as ElementId
    ,noteElement.Instructions as Instructions
    ,noteElement.ImageFileId as ImageFileId
    ,noteElement.RequireText as RequireText
    ,noteElement.RequireImageSelection as RequireImageSelection
from [Content].[NoteElement] noteElement
where 1=1
    and noteElement.IsDeleted = 0
    and noteElement.VersionOf = noteElement.Id