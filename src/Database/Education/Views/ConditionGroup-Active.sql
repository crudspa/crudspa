create view [Education].[ConditionGroup-Active] as

select conditionGroup.Id as Id
    ,conditionGroup.Name as Name
    ,conditionGroup.Ordinal as Ordinal
from [Education].[ConditionGroup] conditionGroup
where 1=1
    and conditionGroup.IsDeleted = 0