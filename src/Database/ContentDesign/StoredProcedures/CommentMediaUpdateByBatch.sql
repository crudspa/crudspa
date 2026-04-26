create proc [ContentDesign].[CommentMediaUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@CommentId uniqueidentifier
    ,@Type int
    ,@AudioId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@VideoId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[CommentMedia]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Type = @Type
    ,AudioId = @AudioId
    ,ImageId = @ImageId
    ,PdfId = @PdfId
    ,VideoId = @VideoId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction