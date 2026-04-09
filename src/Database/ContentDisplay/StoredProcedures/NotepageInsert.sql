create proc [ContentDisplay].[NotepageInsert] (
     @SessionId uniqueidentifier
    ,@NotebookId uniqueidentifier
    ,@NoteId uniqueidentifier
    ,@Text nvarchar(max)
    ,@SelectedImageFileId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @ordinal int = (select count(1) from [Content].[Notepage-Active] where NotebookId = @NotebookId)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[Notepage] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,NotebookId
    ,NoteId
    ,Text
    ,SelectedImageFileId
    ,Ordinal
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@NotebookId
    ,@NoteId
    ,@Text
    ,@SelectedImageFileId
    ,@ordinal
)