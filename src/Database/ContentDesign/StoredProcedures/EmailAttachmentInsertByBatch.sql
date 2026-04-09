create proc [ContentDesign].[EmailAttachmentInsertByBatch] (
     @SessionId uniqueidentifier
    ,@EmailId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[EmailAttachment] (
     Id
    ,EmailId
    ,PdfId
    ,Ordinal
)
values (
     @Id
    ,@EmailId
    ,@PdfId
    ,@Ordinal
)

commit transaction