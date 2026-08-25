create view [Content].[ActivationStagePlan-Active] as

select activationStagePlan.Id as Id
    ,activationStagePlan.ActivationId as ActivationId
    ,activationStagePlan.StageId as StageId
    ,activationStagePlan.ScopeLevel as ScopeLevel
from [Content].[ActivationStagePlan] activationStagePlan
where 1=1
    and activationStagePlan.IsDeleted = 0
    and activationStagePlan.VersionOf = activationStagePlan.Id