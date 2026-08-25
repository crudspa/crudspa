create view [Content].[ActivationStage-Active] as

select activationStage.Id as Id
    ,activationStage.PlanId as PlanId
    ,activationStage.ActivationScopeId as ActivationScopeId
    ,activationStage.Send as Send
    ,activationStage.Overridden as Overridden
from [Content].[ActivationStage] activationStage
where 1=1
    and activationStage.IsDeleted = 0
    and activationStage.VersionOf = activationStage.Id