create view [Content].[Rule-Active] as

select ruleTable.Id as Id
    ,ruleTable.Name as Name
    ,ruleTable.[Key] as [Key]
    ,ruleTable.TypeId as TypeId
    ,ruleTable.DefaultValue as DefaultValue
from [Content].[Rule] ruleTable
where 1=1
    and ruleTable.IsDeleted = 0