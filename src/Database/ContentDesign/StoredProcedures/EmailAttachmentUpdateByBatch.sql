create proc [ContentDesign].[EmailAttachmentUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@EmailId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[EmailAttachment]
set
    Id = @Id
    ,PdfId = @PdfId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction