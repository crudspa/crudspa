create proc [ContentMessaging].[CampaignPopulationRefreshSelect] as

set nocount on

select distinct
     membership.PopulationId
    ,membership.OrganizationId
    ,membership.ActivationScopeId
    ,activationRoot.UpdatedBy as SessionId
from [Content].[Activation-Active] activation
    inner join [Content].[Activation] activationRoot on activationRoot.Id = activation.Id
        and activationRoot.VersionOf = activationRoot.Id
    inner join [Content].[ActivationScope-Active] scope on scope.ActivationId = activation.Id
    inner join [Content].[Membership-Active] membership on membership.ActivationScopeId = scope.Id
where activation.StatusId in (
    'a9b01002-7c15-4fb8-b25f-9f5629031002',
    'a9b01003-7c15-4fb8-b25f-9f5629031003',
    'a9b01005-7c15-4fb8-b25f-9f5629031005'
)
    and membership.PopulationId is not null
    and membership.OrganizationId is not null
    and membership.ActivationScopeId is not null
order by membership.PopulationId, membership.OrganizationId, membership.ActivationScopeId