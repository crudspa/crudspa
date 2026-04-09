create proc [ContentDesign].[NoteImageUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@NoteId uniqueidentifier
    ,@ImageFileId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Content].[NoteImage]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,NoteId = @NoteId
    ,ImageFileId = @ImageFileId
    ,Ordinal = @Ordinal
where Id = @Id