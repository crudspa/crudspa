create proc [EducationPublisher].[ObjectiveUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@TrophyImageId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
    ,@BinderTypeId uniqueidentifier
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

declare @binderId uniqueidentifier = (select top 1 BinderId from [Education].[Objective] where Id = @Id)

update [Content].[Binder]
set
     Id = @binderId
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @BinderTypeId
where Id = @binderId

update objective
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,StatusId = @StatusId
    ,TrophyImageId = @TrophyImageId
    ,RequiresAchievementId = @RequiresAchievementId
    ,GeneratesAchievementId = @GeneratesAchievementId
from [Education].[Objective] objective
where objective.Id = @Id

commit transaction