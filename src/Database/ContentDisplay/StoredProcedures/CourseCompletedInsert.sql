create proc [ContentDisplay].[CourseCompletedInsert] (
     @SessionId uniqueidentifier
    ,@CourseId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @contactId uniqueidentifier = (
    select contact.Id
    from [Framework].[Contact-Active] contact
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

-- If they haven't completed it in 3 months we'll add another record. We only look at completions for the current year so they might need to do it every year
if (not exists(select Id from [Content].[CourseCompleted-Active] where CourseId = @CourseId and ContactId = @contactId and Completed < dateadd(month,-3,getdate()) ))
begin
    insert [Content].[CourseCompleted] (
        Id
        ,UpdatedBy
        ,ContactId
        ,CourseId
        ,Completed
    )
    values (
        @Id
        ,@SessionId
        ,@ContactId
        ,@CourseId
        ,@now
    )
end