create proc [ContentDesign].[PageInsert] (
     @SessionId uniqueidentifier
    ,@BinderId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Title nvarchar(150)
    ,@BoxId uniqueidentifier
    ,@StatusId uniqueidentifier
    ,@GuideText nvarchar(max)
    ,@GuideAudioId uniqueidentifier
    ,@ShowNotebook bit
    ,@ShowGuide bit
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Content].[Page-Active] where BinderId = @BinderId)

insert [Content].[Page] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BinderId
    ,TypeId
    ,Title
    ,BoxId
    ,StatusId
    ,GuideText
    ,GuideAudioId
    ,ShowNotebook
    ,ShowGuide
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@BinderId
    ,@TypeId
    ,@Title
    ,@BoxId
    ,@StatusId
    ,@GuideText
    ,@GuideAudioId
    ,@ShowNotebook
    ,@ShowGuide
    ,@ordinal
)

commit transaction