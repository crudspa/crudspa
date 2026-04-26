create proc [ContentDesign].[CommentMediaInsertByBatch] (
     @SessionId uniqueidentifier
    ,@CommentId uniqueidentifier
    ,@Type int
    ,@AudioId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@VideoId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[CommentMedia] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,CommentId
    ,Type
    ,AudioId
    ,ImageId
    ,PdfId
    ,VideoId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@CommentId
    ,@Type
    ,@AudioId
    ,@ImageId
    ,@PdfId
    ,@VideoId
    ,@Ordinal
)

commit transaction