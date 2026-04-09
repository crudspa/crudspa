create proc [EducationPublisher].[ObjectiveDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @lessonId uniqueidentifier = (select top 1 LessonId from [Education].[Objective] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[Objective-Active] where Id = @Id)
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

update objective
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Objective] objective
where objective.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[Objective] baseTable
    inner join [Education].[Objective-Active] objective on objective.Id = baseTable.Id
where objective.LessonId = @lessonId
    and objective.Ordinal > @oldOrdinal

commit transaction