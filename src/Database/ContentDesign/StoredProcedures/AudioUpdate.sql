create proc [ContentDesign].[AudioUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@FileId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update audio
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,FileId = @FileId
from [Content].[AudioElement] audio
where audio.Id = @Id

commit transaction