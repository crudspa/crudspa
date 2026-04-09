create proc [ContentDesign].[VideoUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@FileId uniqueidentifier
    ,@AutoPlay bit
    ,@PosterId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update video
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,FileId = @FileId
    ,AutoPlay = @AutoPlay
    ,PosterId = @PosterId
from [Content].[VideoElement] video
where video.Id = @Id

commit transaction