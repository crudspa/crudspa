create proc [EducationStudent].[StudentUpdateAcceptTerms] (
     @SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

update [Education].[Student]
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,TermsAccepted = @now
where Id = @studentId