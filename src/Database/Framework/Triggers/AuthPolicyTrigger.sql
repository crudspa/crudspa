create trigger [Framework].[AuthPolicyTrigger] on [Framework].[AuthPolicy]
    for update
as

insert [Framework].[AuthPolicy] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,AuthConnectionId
    ,Audience
    ,[Key]
    ,IdleTimeoutMinutes
    ,AbsoluteTimeoutMinutes
    ,Persist
    ,AutoRedirect
    ,Fallback
    ,Enabled
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.AuthConnectionId
    ,deleted.Audience
    ,deleted.[Key]
    ,deleted.IdleTimeoutMinutes
    ,deleted.AbsoluteTimeoutMinutes
    ,deleted.Persist
    ,deleted.AutoRedirect
    ,deleted.Fallback
    ,deleted.Enabled
from deleted