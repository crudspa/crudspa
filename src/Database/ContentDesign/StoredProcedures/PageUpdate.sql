create proc [ContentDesign].[PageUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Title nvarchar(150)
    ,@BoxId uniqueidentifier
    ,@StatusId uniqueidentifier
    ,@GuideText nvarchar(max)
    ,@GuideAudioId uniqueidentifier
    ,@ShowNotebook bit
    ,@ShowGuide bit
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update page
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @TypeId
    ,Title = @Title
    ,BoxId = @BoxId
    ,StatusId = @StatusId
    ,GuideText = @GuideText
    ,GuideAudioId = @GuideAudioId
    ,ShowNotebook = @ShowNotebook
    ,ShowGuide = @ShowGuide
from [Content].[Page] page
where page.Id = @Id

commit transaction