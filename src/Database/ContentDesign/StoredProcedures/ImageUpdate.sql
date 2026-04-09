create proc [ContentDesign].[ImageUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@FileId uniqueidentifier
    ,@HyperlinkUrl nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update image
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,FileId = @FileId
    ,HyperlinkUrl = @HyperlinkUrl
from [Content].[ImageElement] image
where image.Id = @Id

commit transaction