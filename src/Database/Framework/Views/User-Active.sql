create view [Framework].[User-Active] as

select userTable.Id as Id
    ,userTable.ContactId as ContactId
    ,userTable.PortalId as PortalId
    ,userTable.OrganizationId as OrganizationId
    ,userTable.Username as Username
    ,userTable.PasswordSalt as PasswordSalt
    ,userTable.PasswordHash as PasswordHash
    ,userTable.ResetPassword as ResetPassword
from [Framework].[User] userTable
where 1=1
    and userTable.IsDeleted = 0
    and userTable.VersionOf = userTable.Id