create view [Content].[RuleType-Active] as

select ruleType.Id as Id
    ,ruleType.Name as Name
    ,ruleType.EditorView as EditorView
    ,ruleType.DisplayView as DisplayView
from [Content].[RuleType] ruleType
where 1=1
    and ruleType.IsDeleted = 0