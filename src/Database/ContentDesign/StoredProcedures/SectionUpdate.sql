create proc [ContentDesign].[SectionUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@BoxId uniqueidentifier
    ,@ContainerId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update section
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @TypeId
    ,BoxId = @BoxId
    ,ContainerId = @ContainerId
from [Content].[Section] section
where section.Id = @Id

commit transaction