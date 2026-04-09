create proc [EducationPublisher].[ActivityInsert] (
     @SessionId uniqueidentifier
    ,@ActivityTypeId uniqueidentifier
    ,@ContentAreaId uniqueidentifier
    ,@ContextText nvarchar(max)
    ,@ContextAudioFileId uniqueidentifier
    ,@ContextImageFileId uniqueidentifier
    ,@ExtraText nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[Activity] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ActivityTypeId
    ,ContentAreaId
    ,StatusId
    ,ContextText
    ,ContextAudioFileId
    ,ContextImageFileId
    ,ExtraText
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ActivityTypeId
    ,@ContentAreaId
    ,'2ad6b3d0-3381-4c67-b6f7-5b0c0822dc9d' -- Complete
    ,@ContextText
    ,@ContextAudioFileId
    ,@ContextImageFileId
    ,@ExtraText
)

commit transaction