create proc [ContentDisplay].[CourseProgressSelect] (
     @SessionId uniqueidentifier
    ,@CourseId uniqueidentifier
) as

declare @contactId uniqueidentifier = (
    select contact.Id
    from [Framework].[Contact-Active] contact
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

;with CourseCompletedCte(CourseId, EventCount) as (
    select courseCompleted.CourseId, count(1) as CourseCompletedCount
    from [Content].[CourseCompleted] courseCompleted
    where courseCompleted.CourseId = @CourseId
        and courseCompleted.ContactId = @contactId
    group by courseCompleted.CourseId
)

select
    @ContactId as ContactId
    ,course.Id as CourseId
    ,isnull(coursesCompleted.EventCount, 0) as TimesCompleted
from [Content].[Course-Active] course
    left join CourseCompletedCte coursesCompleted on course.Id = coursesCompleted.CourseId
where coursesCompleted.EventCount is not null