create proc [ContentDisplay].[ContactNotebookSelectForContact] (
     @SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @contactNotebookId uniqueidentifier
declare @notebookId uniqueidentifier

if not exists (
    select 1
    from [Content].[ContactNotebook-Active]
    where ContactId = @contactId)
begin

    set @contactNotebookId = newid()
    set @notebookId = newid()

    insert [Content].[Notebook] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy

    )
    values (
        @notebookId
        ,@notebookId
        ,@now
        ,@SessionId
    )

    insert [Content].[ContactNotebook] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,ContactId
        ,NotebookId
    )
    values (
        @contactNotebookId
        ,@contactNotebookId
        ,@now
        ,@SessionId
        ,@contactId
        ,@notebookId
    )
end
else
begin

    select top 1
        @contactNotebookId = contactNotebook.Id
        ,@notebookId =  contactNotebook.NotebookId
    from [Content].[ContactNotebook] contactNotebook
    where contactNotebook.IsDeleted = 0
        and contactNotebook.VersionOf = contactNotebook.Id
        and contactNotebook.ContactId = @contactId
    order by contactNotebook.Updated desc

end


select
    contactNotebook.Id as Id
    ,contactNotebook.ContactId as ContactId
    ,contactNotebook.NotebookId as NotebookId
from [Content].[ContactNotebook] contactNotebook
where contactNotebook.Id = @contactNotebookId

select
    notepage.Id as Id
    ,notepage.NotebookId as NotebookId
    ,notepage.NoteId as NoteId
    ,notepage.Text as Text
    ,notepage.SelectedImageFileId as SelectedImageFileId
    ,notepage.Ordinal as Ordinal
    ,note.Id as NoteNoteId
    ,note.Instructions as NoteInstructions
    ,noteImageFile.Id as FileId
    ,noteImageFile.BlobId as FileBlobId
    ,noteImageFile.Name as FileName
    ,noteImageFile.Format as FileFormat
    ,noteImageFile.Width as FileWidth
    ,noteImageFile.Height as FileHeight
    ,noteImageFile.Caption as FileCaption
    ,note.RequireText as NoteRequireText
    ,note.RequireImageSelection as NoteRequireImageSelection
    ,noteImageFile.Id as NoteImageFileId
    ,noteImageFile.BlobId as NoteImageFileBlobId
    ,noteImageFile.Name as NoteImageFileName
    ,noteImageFile.Format as NoteImageFileFormat
    ,noteImageFile.Width as NoteImageFileWidth
    ,noteImageFile.Height as NoteImageFileHeight
    ,noteImageFile.Caption as NoteImageFileCaption
    ,selectedImageFile.Id as FileId
    ,selectedImageFile.BlobId as FileBlobId
    ,selectedImageFile.Name as FileName
    ,selectedImageFile.Format as FileFormat
    ,selectedImageFile.Width as FileWidth
    ,selectedImageFile.Height as FileHeight
    ,selectedImageFile.Caption as FileCaption
from [Content].[Notepage-Active] notepage
    inner join [Content].[NoteElement-Active] note on notepage.NoteId = note.Id
    left join [Framework].[ImageFile-Active] noteImageFile on note.ImageFileId = noteImageFile.Id
    left join [Framework].[ImageFile-Active] selectedImageFile on notepage.SelectedImageFileId = selectedImageFile.Id
where notepage.NotebookId = @notebookId
order by notepage.Ordinal asc

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
    inner join [Content].[NoteElement-Active] note on noteImage.NoteId = note.Id
    inner join [Content].[Notepage-Active] notepage on notepage.NoteId = note.Id
    left join [Framework].[ImageFile-Active] imageFile on noteImage.ImageFileId = imageFile.Id
where notepage.NotebookId = @notebookId
order by noteImage.Ordinal asc