create proc [EducationStudent].[LessonCompletedInsert] (
     @SessionId uniqueidentifier
    ,@ObjectiveId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @lessonId uniqueidentifier = (select top 1 LessonId from [Education].[Objective-Active] where Id = @ObjectiveId)

if (not exists(select Id from [Education].[LessonCompleted-Active] where LessonId = @lessonId and StudentId = @studentId))
begin
    insert [Education].[LessonCompleted] (
        Id
        ,UpdatedBy
        ,StudentId
        ,LessonId
        ,Completed
    )
    values (
        @Id
        ,@SessionId
        ,@studentId
        ,@lessonId
        ,@now
    )
end