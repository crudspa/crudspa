create proc [ContentMessaging].[PopulationRefreshForActivationSelect] (
     @SessionId uniqueidentifier
    ,@ActivationId uniqueidentifier
) as

set nocount on

select distinct
     membership.PopulationId
    ,membership.OrganizationId
    ,membership.ActivationScopeId
from [Content].[Activation-Active] activation
    inner join [Content].[ActivationScope-Active] scope on scope.ActivationId = activation.Id
    inner join [Content].[Membership-Active] membership on membership.ActivationScopeId = scope.Id
    inner join [Content].[Campaign-Active] campaign on campaign.Id = activation.CampaignId
    cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, campaign.PortalId, activation.OrganizationId)
where activation.Id = @ActivationId
    and membership.PopulationId is not null
    and membership.OrganizationId is not null
    and membership.ActivationScopeId is not null