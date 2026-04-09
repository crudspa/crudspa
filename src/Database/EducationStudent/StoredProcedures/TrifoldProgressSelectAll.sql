create proc [EducationStudent].[TrifoldProgressSelectAll] (
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

;with TrifoldCompletedCte(TrifoldId, EventCount) as (
    select trifoldCompleted.TrifoldId, count(1) as TrifoldCompletedCount
    from [Education].[TrifoldCompleted] trifoldCompleted
    where trifoldCompleted.StudentId = @studentId
    group by trifoldCompleted.TrifoldId
)

select
    @StudentId as StudentId
    ,trifold.Id as TrifoldId
    ,isnull(trifoldsCompleted.EventCount, 0) as TimesCompleted
from [Education].[Trifold-Active] trifold
    left join TrifoldCompletedCte trifoldsCompleted on trifold.Id = trifoldsCompleted.TrifoldId
where trifoldsCompleted.EventCount is not null