create proc [EducationPublisher].[ActivityUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ActivityTypeId uniqueidentifier
    ,@ContentAreaId uniqueidentifier
    ,@ContextText nvarchar(max)
    ,@ContextAudioFileId uniqueidentifier
    ,@ContextImageFileId uniqueidentifier
    ,@ExtraText nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update activity
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,ActivityTypeId = @ActivityTypeId
    ,ContentAreaId = @ContentAreaId
    ,ContextText = @ContextText
    ,ContextAudioFileId = @ContextAudioFileId
    ,ContextImageFileId = @ContextImageFileId
    ,ExtraText = @ExtraText
from [Education].[Activity] activity

where activity.Id = @Id


commit transaction