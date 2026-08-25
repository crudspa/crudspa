create view [Content].[ActivationScope-Active] as

select activationScope.Id as Id
    ,activationScope.ActivationId as ActivationId
    ,activationScope.ParentId as ParentId
    ,activationScope.OrganizationId as OrganizationId
    ,activationScope.Name as Name
    ,activationScope.Start as Start
    ,activationScope.StartOverridden as StartOverridden
    ,activationScope.Ordinal as Ordinal
from [Content].[ActivationScope] activationScope
where 1=1
    and activationScope.IsDeleted = 0
    and activationScope.VersionOf = activationScope.Id