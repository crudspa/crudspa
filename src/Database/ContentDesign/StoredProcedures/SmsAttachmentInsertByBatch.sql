create proc [ContentDesign].[SmsAttachmentInsertByBatch] (
     @SessionId uniqueidentifier
    ,@SmsId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[SmsAttachment] (
     Id
    ,SmsId
    ,ImageId
    ,Ordinal
)
values (
     @Id
    ,@SmsId
    ,@ImageId
    ,@Ordinal
)

commit transaction