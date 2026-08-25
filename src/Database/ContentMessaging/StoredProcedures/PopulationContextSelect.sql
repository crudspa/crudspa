create proc [ContentMessaging].[PopulationContextSelect] (
     @SessionId uniqueidentifier
    ,@PopulationId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@ActivationScopeId uniqueidentifier = null
) as

declare @accessOrganizationId uniqueidentifier = @OrganizationId

if @ActivationScopeId is not null
begin
    set @accessOrganizationId = null

    select @accessOrganizationId = activation.OrganizationId
    from [Content].[ActivationScope-Active] scope
        inner join [Content].[Activation-Active] activation on activation.Id = scope.ActivationId
    where scope.Id = @ActivationScopeId
        and scope.OrganizationId = @OrganizationId
end

set nocount on

select
     population.Id
    ,population.PortalId
    ,population.[Key]
    ,population.Name
    ,population.Description
    ,population.SupportsOptOut
    ,population.ResolverKey
    ,membership.Id as MembershipId
from [Content].[Population-Active] population
    inner join [Framework].[Portal-Active] portal on population.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on organization.Id = @OrganizationId
    cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, population.PortalId, @accessOrganizationId)
    left join [Content].[Membership-Active] membership on membership.PopulationId = population.Id
        and membership.OrganizationId = organization.Id
        and ((membership.ActivationScopeId = @ActivationScopeId) or (membership.ActivationScopeId is null and @ActivationScopeId is null))
where population.Id = @PopulationId
    and (@ActivationScopeId is null or @accessOrganizationId is not null)