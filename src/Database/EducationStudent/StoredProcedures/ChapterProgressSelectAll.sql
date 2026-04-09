create proc [EducationStudent].[ChapterProgressSelectAll] (
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

;with ChapterCompletedCte(ChapterId, EventCount) as (
    select chapterCompleted.ChapterId, count(1) as ChapterCompletedCount
    from [Education].[ChapterCompleted] chapterCompleted
    where chapterCompleted.StudentId = @studentId
    group by chapterCompleted.ChapterId
)

select
    @StudentId as StudentId
    ,chapter.Id as ChapterId
    ,isnull(chaptersCompleted.EventCount, 0) as TimesCompleted
from [Education].[Chapter-Active] chapter
    left join ChapterCompletedCte chaptersCompleted on chapter.Id = chaptersCompleted.ChapterId
where chaptersCompleted.EventCount is not null