create proc [EducationStudent].[StudentUpdateProfile] (
     @SessionId uniqueidentifier
    ,@AvatarString nvarchar(2)
    ,@PreferredName nvarchar(75)
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[Student]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,AvatarString = @AvatarString
    ,PreferredName = @PreferredName
where Id = @studentId