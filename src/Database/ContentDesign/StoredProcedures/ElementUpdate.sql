create proc [ContentDesign].[ElementUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@RequireInteraction bit
    ,@BoxId uniqueidentifier
    ,@ItemId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update element
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @TypeId
    ,RequireInteraction = @RequireInteraction
    ,BoxId = @BoxId
    ,ItemId = @ItemId
    ,Ordinal = @Ordinal
from [Content].[Element] element
where element.Id = @Id

commit transaction