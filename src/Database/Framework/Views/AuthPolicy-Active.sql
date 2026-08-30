create view [Framework].[AuthPolicy-Active] as

select authPolicy.Id as Id
    ,authPolicy.OrganizationId as OrganizationId
    ,authPolicy.AuthConnectionId as AuthConnectionId
    ,authPolicy.Audience as Audience
    ,authPolicy.[Key] as [Key]
    ,authPolicy.IdleTimeoutMinutes as IdleTimeoutMinutes
    ,authPolicy.AbsoluteTimeoutMinutes as AbsoluteTimeoutMinutes
    ,authPolicy.Persist as Persist
    ,authPolicy.AutoRedirect as AutoRedirect
    ,authPolicy.Fallback as Fallback
    ,authPolicy.Enabled as Enabled
from [Framework].[AuthPolicy] authPolicy
where 1=1
    and authPolicy.IsDeleted = 0
    and authPolicy.VersionOf = authPolicy.Id