create view [Content].[Activation-Active] as

select activation.Id as Id
    ,activation.BatchId as BatchId
    ,activation.CampaignId as CampaignId
    ,activation.OrganizationId as OrganizationId
    ,activation.Start as Start
    ,activation.Activated as Activated
    ,activation.ActivatedBy as ActivatedBy
    ,activation.StatusId as StatusId
from [Content].[Activation] activation
where 1=1
    and activation.IsDeleted = 0
    and activation.VersionOf = activation.Id