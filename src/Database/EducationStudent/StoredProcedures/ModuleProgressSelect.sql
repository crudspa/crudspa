create proc [EducationStudent].[ModuleProgressSelect] (
     @SessionId uniqueidentifier
    ,@ModuleId uniqueidentifier
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

;with ModulesCompletedCte(ModuleId, EventCount) as (
    select moduleCompleted.ModuleId, count(1) as ModuleCompletedCount
    from [Education].[ModuleCompleted] moduleCompleted
    where moduleCompleted.StudentId = @studentId
        and moduleCompleted.ModuleId = @ModuleId
    group by moduleCompleted.ModuleId
)

select
    @StudentId as StudentId
    ,module.Id as ModuleId
    ,module.BookId as BookId
    ,isnull(modulesCompleted.EventCount, 0) as TimesCompleted
from [Education].[Module-Active] module
    left join ModulesCompletedCte modulesCompleted on module.Id = modulesCompleted.ModuleId
where modulesCompleted.EventCount is not null