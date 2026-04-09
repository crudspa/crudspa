create proc [FrameworkCore].[UserSelectByUsername] (
     @PortalId uniqueidentifier
    ,@Username nvarchar(75)
) as

declare @userId uniqueidentifier = (
    select top 1 Id
    from [Framework].[User-Active] userTable
    where userTable.Username = @Username
        and userTable.PortalId = @PortalId
)

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