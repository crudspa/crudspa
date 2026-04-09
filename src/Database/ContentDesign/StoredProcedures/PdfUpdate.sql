create proc [ContentDesign].[PdfUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@FileId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update pdf
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,FileId = @FileId
from [Content].[PdfElement] pdf
where pdf.Id = @Id

commit transaction