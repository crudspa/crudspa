create proc [FrameworkCore].[UserSelectBySession] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

declare @userId uniqueidentifier = (select top 1 UserId from [Framework].[Session-Active] where Id = @SessionId and PortalId = @PortalId)

select distinct
     userTable.Id
    ,userTable.PortalId
    ,userTable.Username
    ,userTable.PasswordHash
    ,userTable.PasswordSalt
    ,userTable.ResetPassword
    ,contact.Id as ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
    ,contact.TimeZoneId as ContactTimeZoneId
from [Framework].[User-Active] userTable
    inner join [Framework].[Contact-Active] contact on userTable.ContactId = contact.Id
    inner join [Framework].[Organization-Active] organization on userTable.OrganizationId = organization.Id
where userTable.Id = @userId

select
     contactEmail.Id
    ,contactEmail.ContactId
    ,contactEmail.Email
    ,contactEmail.Ordinal
from [Framework].[User-Active] userTable
    inner join [Framework].[Contact-Active] contact on userTable.ContactId = contact.Id
    inner join [Framework].[ContactEmail-Active] contactEmail on contactEmail.ContactId = contact.Id
where userTable.Id = @userId
order by contactEmail.Ordinal, contactEmail.Email