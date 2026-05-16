create proc [ContentDesign].[NoteUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Instructions nvarchar(max)
    ,@ImageFileId uniqueidentifier
    ,@RequireText bit
    ,@RequireImageSelection bit
    ,@OpenOnly bit
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update note
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Instructions = @Instructions
    ,ImageFileId = @ImageFileId
    ,RequireText = @RequireText
    ,RequireImageSelection = @RequireImageSelection
    ,OpenOnly = @OpenOnly
from [Content].[NoteElement] note
where note.Id = @Id

commit transaction