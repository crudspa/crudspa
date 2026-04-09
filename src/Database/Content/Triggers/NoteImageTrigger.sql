create trigger [Content].[NoteImageTrigger] on [Content].[NoteImage]
    for update
as

insert [Content].[NoteImage] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,NoteId
    ,ImageFileId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.NoteId
    ,deleted.ImageFileId
    ,deleted.Ordinal
from deleted