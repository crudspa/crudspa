create view [Content].[NoteImage-Active] as

select noteImage.Id as Id
    ,noteImage.NoteId as NoteId
    ,noteImage.ImageFileId as ImageFileId
    ,noteImage.Ordinal as Ordinal
from [Content].[NoteImage] noteImage
where 1=1
    and noteImage.IsDeleted = 0
    and noteImage.VersionOf = noteImage.Id