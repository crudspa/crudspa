create trigger [Content].[NotepageTrigger] on [Content].[Notepage]
    for update
as

insert [Content].[Notepage] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,NotebookId
    ,NoteId
    ,Text
    ,SelectedImageFileId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.NotebookId
    ,deleted.NoteId
    ,deleted.Text
    ,deleted.SelectedImageFileId
    ,deleted.Ordinal
from deleted