create proc [EducationPublisher].[LessonDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @unitId uniqueidentifier = (select top 1 UnitId from [Education].[Lesson] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[Lesson-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

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

update lesson
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Lesson] lesson
where lesson.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[Lesson] baseTable
    inner join [Education].[Lesson-Active] lesson on lesson.Id = baseTable.Id
where lesson.UnitId = @unitId
    and lesson.Ordinal > @oldOrdinal

commit transaction