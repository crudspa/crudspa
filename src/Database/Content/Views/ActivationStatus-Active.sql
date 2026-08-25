create view [Content].[ActivationStatus-Active] as

select activationStatus.Id as Id
    ,activationStatus.Name as Name
    ,activationStatus.Ordinal as Ordinal
from [Content].[ActivationStatus] activationStatus
where 1=1
    and activationStatus.IsDeleted = 0