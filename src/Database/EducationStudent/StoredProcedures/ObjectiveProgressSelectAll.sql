create proc [EducationStudent].[ObjectiveProgressSelectAll] (
     @SessionId uniqueidentifier
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

;with ObjectiveCompletedCte(ObjectiveId, EventCount) as (
    select objectiveCompleted.ObjectiveId, count(1) as ObjectiveCompletedCount
    from [Education].[ObjectiveCompleted] objectiveCompleted
    where objectiveCompleted.StudentId = @studentId
    group by objectiveCompleted.ObjectiveId
)

select
    @StudentId as StudentId
    ,objective.Id as ObjectiveId
    ,isnull(objectivesCompleted.EventCount, 0) as TimesCompleted
from [Education].[Objective-Active] objective
    left join ObjectiveCompletedCte objectivesCompleted on objective.Id = objectivesCompleted.ObjectiveId
where objectivesCompleted.EventCount is not null