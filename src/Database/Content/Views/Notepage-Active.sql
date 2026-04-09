create view [Content].[Notepage-Active] as

select notepage.Id as Id
    ,notepage.NotebookId as NotebookId
    ,notepage.NoteId as NoteId
    ,notepage.Text as Text
    ,notepage.SelectedImageFileId as SelectedImageFileId
    ,notepage.Ordinal as Ordinal
from [Content].[Notepage] notepage
where 1=1
    and notepage.IsDeleted = 0
    and notepage.VersionOf = notepage.Id