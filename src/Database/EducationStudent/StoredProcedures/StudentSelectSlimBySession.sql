create proc [EducationStudent].[StudentSelectSlimBySession] (
     @SessionId uniqueidentifier
) as

select
    student.Id
    ,student.ContactId
    ,student.UserId
    ,contact.FirstName
    ,contact.LastName
    ,student.GenderId
    ,student.GradeId
    ,student.AudioGenderId
    ,student.ChallengeLevelId
    ,student.ResearchGroupId
    ,coalesce(student.PreferredName, contact.FirstName) as PreferredName
    ,coalesce(student.AvatarString, nchar(0xD83D) + nchar(0xDC39)) as AvatarString
    ,student.TermsAccepted
    ,student.IsTestAccount
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
where session.Id = @SessionId