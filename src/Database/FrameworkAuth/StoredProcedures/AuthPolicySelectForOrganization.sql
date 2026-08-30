create proc [FrameworkAuth].[AuthPolicySelectForOrganization] (
     @OrganizationId uniqueidentifier
) as

select
     authPolicy.Id
    ,authPolicy.OrganizationId
    ,authPolicy.AuthConnectionId
    ,authPolicy.Audience
    ,authPolicy.[Key]
    ,authPolicy.IdleTimeoutMinutes
    ,authPolicy.AbsoluteTimeoutMinutes
    ,authPolicy.Persist
    ,authPolicy.AutoRedirect
    ,authPolicy.Fallback
    ,authPolicy.Enabled
from [Framework].[AuthPolicy-Active] authPolicy
where authPolicy.OrganizationId = @OrganizationId
order by authPolicy.Audience