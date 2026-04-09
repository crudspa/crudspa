create proc [EducationStudent].[ChapterAllAreComplete] (
     @SessionId uniqueidentifier
    ,@ChapterId uniqueidentifier
    ,@AllAreComplete bit output
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @bookId uniqueidentifier = (select BookId from [Education].[Chapter-Active] where Id = @ChapterId)

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

if (exists(
    select chapter.Id
    from [Education].[Chapter-Active] chapter
    where chapter.BookId = @bookId
        and chapter.Id not in (select ChapterId from [Education].[ChapterCompleted] where StudentId = @studentId)
))
begin
    set @AllAreComplete = 0
end
else
begin
    set @AllAreComplete = 1
end