create proc [ContentDesign].[AnswerTypeSelectFull] as

select
     answerType.Id
    ,answerType.Name
    ,answerType.DesignView
    ,answerType.DisplayView
from [Content].[AnswerType-Active] answerType
order by answerType.Name