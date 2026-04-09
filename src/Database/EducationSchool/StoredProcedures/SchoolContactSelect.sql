create proc [EducationSchool].[SchoolContactSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     schoolContact.Id
    ,schoolContact.TitleId
    ,title.Name as TitleName
    ,schoolContact.TestAccount
    ,schoolContact.ContactId
    ,schoolContact.UserId
from [Education].[SchoolContact-Active] schoolContact
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    inner join [Education].[Title-Active] title on schoolContact.TitleId = title.Id
    left join [Framework].[User-Active] userTable on schoolContact.UserId = userTable.Id
where schoolContact.Id = @Id
    and school.OrganizationId = @organizationId
    and school.Id = @schoolId