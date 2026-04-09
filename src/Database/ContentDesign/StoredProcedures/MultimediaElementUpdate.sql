create proc [ContentDesign].[MultimediaElementUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update multimediaElement
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[MultimediaElement] multimediaElement
where multimediaElement.Id = @Id

commit transaction