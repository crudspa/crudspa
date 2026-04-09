create proc [EducationPublisher].[LessonUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,StatusId = @StatusId
    ,ImageId = @ImageId
    ,GuideImageId = @GuideImageId
    ,GuideText = @GuideText
    ,GuideAudioId = @GuideAudioId
    ,RequiresAchievementId = @RequiresAchievementId
    ,RequireSequentialCompletion = @RequireSequentialCompletion
    ,Treatment = @Treatment
    ,Control = @Control
from [Education].[Lesson] baseTable
    inner join [Education].[Lesson-Active] lesson on lesson.Id = baseTable.Id
    inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction