create proc [ContentDesign].[NoteImageSelectForNote] (
     @NoteId uniqueidentifier
) as

select
    noteImage.Id as Id
    ,noteImage.NoteId as NoteId
    ,noteImage.ImageFileId as ImageFileId
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,noteImage.Ordinal
from [Content].[NoteImage-Active] noteImage
    left join [Framework].[ImageFile-Active] imageFile on noteImage.ImageFileId = imageFile.Id
where noteImage.NoteId = @NoteId