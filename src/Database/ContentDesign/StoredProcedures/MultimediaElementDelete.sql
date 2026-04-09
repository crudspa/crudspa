create proc [ContentDesign].[MultimediaElementDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update multimediaElement
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[MultimediaElement] multimediaElement
where multimediaElement.Id = @Id

commit transaction