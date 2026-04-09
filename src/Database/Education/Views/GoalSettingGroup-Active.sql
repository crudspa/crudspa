create view [Education].[GoalSettingGroup-Active] as

select goalSettingGroup.Id as Id
    ,goalSettingGroup.Name as Name
    ,goalSettingGroup.Ordinal as Ordinal
from [Education].[GoalSettingGroup] goalSettingGroup
where 1=1
    and goalSettingGroup.IsDeleted = 0