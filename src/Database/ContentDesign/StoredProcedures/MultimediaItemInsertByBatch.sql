create proc [ContentDesign].[MultimediaItemInsertByBatch] (
     @SessionId uniqueidentifier
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
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[MultimediaItem] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MultimediaElementId
    ,BoxId
    ,ItemId
    ,MediaTypeIndex
    ,AudioId
    ,ButtonId
    ,ImageId
    ,Text
    ,VideoId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MultimediaElementId
    ,@BoxId
    ,@ItemId
    ,@MediaTypeIndex
    ,@AudioId
    ,@ButtonId
    ,@ImageId
    ,@Text
    ,@VideoId
    ,@Ordinal
)

commit transaction