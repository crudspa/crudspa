create proc [ContentMessaging].[EmailMembershipSelectForActivation] (
     @SessionId uniqueidentifier
    ,@ActivationId uniqueidentifier
) as

set nocount on

select distinct
     membership.Id
    ,membership.Name
from [Content].[Activation-Active] activation
    inner join [Content].[Campaign-Active] campaign on campaign.Id = activation.CampaignId
    inner join [Content].[ActivationScope-Active] scope on scope.ActivationId = activation.Id
    inner join [Content].[Membership-Active] membership on membership.ActivationScopeId = scope.Id
    cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, campaign.PortalId, activation.OrganizationId)
where activation.Id = @ActivationId
order by membership.Name, membership.Id