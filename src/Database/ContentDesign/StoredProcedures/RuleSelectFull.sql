create proc [ContentDesign].[RuleSelectFull] as

select
    ruleTable.Id
    ,ruleTable.Name
    ,ruleTable.[Key]
    ,ruleTable.TypeId
    ,ruleTable.DefaultValue
    ,ruleType.Id as RuleTypeId
    ,ruleType.Name as RuleTypeName
    ,ruleType.EditorView as RuleTypeEditorView
    ,ruleType.DisplayView as RuleTypeDisplayView
from [Content].[Rule-Active] ruleTable
    inner join [Content].[RuleType-Active] type on ruleTable.TypeId = type.Id
    inner join [Content].[RuleType-Active] ruleType on ruleTable.TypeId = ruleType.Id
order by ruleTable.Name