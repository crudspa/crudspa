create proc [EducationPublisher].[LessonInsert] (
     @SessionId uniqueidentifier
    ,@UnitId uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@GuideImageId uniqueidentifier
    ,@GuideText nvarchar(max)
    ,@GuideAudioId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@RequireSequentialCompletion bit
    ,@Treatment bit
    ,@Control bit
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Education].[Lesson-Active] where UnitId = @UnitId)

insert [Education].[Lesson] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,UnitId
    ,Title
    ,StatusId
    ,ImageId
    ,GuideImageId
    ,GuideText
    ,GuideAudioId
    ,RequiresAchievementId
    ,RequireSequentialCompletion
    ,Treatment
    ,Control
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@UnitId
    ,@Title
    ,@StatusId
    ,@ImageId
    ,@GuideImageId
    ,@GuideText
    ,@GuideAudioId
    ,@RequiresAchievementId
    ,@RequireSequentialCompletion
    ,@Treatment
    ,@Control
    ,@ordinal
)

if not exists (
    select 1
    from [Education].[Lesson-Active] lesson
        inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where lesson.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction