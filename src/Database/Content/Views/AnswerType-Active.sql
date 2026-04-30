create view [Content].[AnswerType-Active] as

select answerType.Id as Id
    ,answerType.Name as Name
    ,answerType.DesignView as DesignView
    ,answerType.DisplayView as DisplayView
from [Content].[AnswerType] answerType
where 1=1
    and answerType.IsDeleted = 0