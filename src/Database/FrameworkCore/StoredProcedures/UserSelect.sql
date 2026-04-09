create proc [FrameworkCore].[UserSelect] (
     @Id uniqueidentifier
) as

set nocount on

declare @organizationId uniqueidentifier = (select top 1 userTable.OrganizationId from [Framework].[User-Active] userTable where userTable.Id = @Id)

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
where userTable.Id = @Id

select
     contactEmail.Id
    ,contactEmail.ContactId
    ,contactEmail.Email
    ,contactEmail.Ordinal
from [Framework].[User-Active] userTable
    inner join [Framework].[Contact-Active] contact on userTable.ContactId = contact.Id
    inner join [Framework].[ContactEmail-Active] contactEmail on contactEmail.ContactId = contact.Id
where userTable.Id = @Id
order by contactEmail.Ordinal, contactEmail.Email

select distinct
     @Id as UserId
    ,role.Id as RoleId
    ,role.Name as RoleName
    ,convert(bit, iif(userRole.Id is null, 0, 1)) as Selected
from [Framework].[Role-Active] role
    left join [Framework].[UserRole-Active] userRole on userRole.RoleId = role.Id
        and userRole.UserId = @Id
where role.OrganizationId = @organizationId
order by role.Name