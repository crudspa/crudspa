create proc [EducationPublisher].[ObjectiveInsert] (
     @SessionId uniqueidentifier
    ,@LessonId uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@TrophyImageId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
    ,@BinderTypeId uniqueidentifier
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

declare @ordinal int = (select count(1) from [Education].[Objective-Active] where LessonId = @LessonId)

declare @binderId uniqueidentifier = newid()

insert [Content].[Binder] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,TypeId
)
values (
     @binderId
    ,@binderId
    ,@now
    ,@SessionId
    ,@BinderTypeId
)

insert [Education].[Objective] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LessonId
    ,Title
    ,StatusId
    ,TrophyImageId
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,BinderId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LessonId
    ,@Title
    ,@StatusId
    ,@TrophyImageId
    ,@RequiresAchievementId
    ,@GeneratesAchievementId
    ,@binderId
    ,@ordinal
)

if not exists (
    select 1
    from [Education].[Objective-Active] objective
        inner join [Education].[Lesson-Active] lesson on objective.LessonId = lesson.Id
        inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where objective.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction