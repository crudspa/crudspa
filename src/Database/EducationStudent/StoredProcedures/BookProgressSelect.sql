create proc [EducationStudent].[BookProgressSelect] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

;with PrefaceCompletedCte(BookId, EventCount) as (
    select prefaceCompleted.BookId, count(1) as PrefaceCompletedCount
    from [Education].[PrefaceCompleted] prefaceCompleted
    where prefaceCompleted.StudentId = @studentId
        and prefaceCompleted.BookId = @BookId
    group by prefaceCompleted.BookId
),
ContentCompletedCte(BookId, EventCount) as (
    select contentCompleted.BookId, count(1) as ContentCompletedCount
    from [Education].[ContentCompleted] contentCompleted
    where contentCompleted.StudentId = @studentId
        and contentCompleted.BookId = @BookId
    group by contentCompleted.BookId
),
MapCompletedCte(BookId, EventCount) as (
    select mapCompleted.BookId, count(1) as MapCompletedCount
    from [Education].[MapCompleted] mapCompleted
    where mapCompleted.StudentId = @studentId
        and mapCompleted.BookId = @BookId
    group by mapCompleted.BookId
)

select
    @studentId as StudentId
    ,book.Id as BookId
    ,isnull(prefacesCompleted.EventCount, 0) as PrefacesCompleted
    ,isnull(contentsCompleted.EventCount, 0) as ContentsCompleted
    ,isnull(mapCompleted.EventCount, 0) as MapCompleted
from [Education].[Book-Active] book
    left join PrefaceCompletedCte prefacesCompleted on book.Id = prefacesCompleted.BookId
    left join ContentCompletedCte contentsCompleted on book.Id = contentsCompleted.BookId
    left join MapCompletedCte mapCompleted on book.Id = mapCompleted.BookId
where prefacesCompleted.EventCount is not null
    or contentsCompleted.EventCount is not null
    or mapCompleted.EventCount is not null