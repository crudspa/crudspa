create proc [ContentDesign].[TextElementUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Text nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update textElement
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Text = @Text
from [Content].[TextElement] textElement
where textElement.Id = @Id

commit transaction