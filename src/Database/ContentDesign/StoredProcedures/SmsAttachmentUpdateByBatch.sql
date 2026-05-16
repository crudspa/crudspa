create proc [ContentDesign].[SmsAttachmentUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@SmsId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[SmsAttachment]
set
    Id = @Id
    ,ImageId = @ImageId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction