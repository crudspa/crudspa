create view [Education].[PersonalizationGroup-Active] as

select personalizationGroup.Id as Id
    ,personalizationGroup.Name as Name
    ,personalizationGroup.Ordinal as Ordinal
from [Education].[PersonalizationGroup] personalizationGroup
where 1=1
    and personalizationGroup.IsDeleted = 0