create proc [ContentDesign].[EmailAttachmentDeleteByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set xact_abort on
set nocount on
begin transaction

update [Content].[EmailAttachment]
set  IsDeleted = 1
where Id = @Id

commit transaction