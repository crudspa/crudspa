create proc [ContentDesign].[MultimediaItemUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@MultimediaElementId uniqueidentifier
    ,@BoxId uniqueidentifier
    ,@ItemId uniqueidentifier
    ,@MediaTypeIndex int
    ,@AudioId uniqueidentifier
    ,@ButtonId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@Text nvarchar(max)
    ,@VideoId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[MultimediaItem]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,BoxId = @BoxId
    ,ItemId = @ItemId
    ,MediaTypeIndex = @MediaTypeIndex
    ,AudioId = @AudioId
    ,ButtonId = @ButtonId
    ,ImageId = @ImageId
    ,Text = @Text
    ,VideoId = @VideoId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction