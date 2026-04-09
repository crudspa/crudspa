create trigger [Framework].[UserTrigger] on [Framework].[User]
    for update
as

insert [Framework].[User] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,PortalId
    ,OrganizationId
    ,Username
    ,PasswordSalt
    ,PasswordHash
    ,ResetPassword
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.PortalId
    ,deleted.OrganizationId
    ,deleted.Username
    ,deleted.PasswordSalt
    ,deleted.PasswordHash
    ,deleted.ResetPassword
from deleted