create view [Content].[QuestionElement-Active] as

select questionElement.Id as Id
    ,questionElement.ElementId as ElementId
    ,questionElement.QuestionId as QuestionId
from [Content].[QuestionElement] questionElement
where 1=1
    and questionElement.IsDeleted = 0
    and questionElement.VersionOf = questionElement.Id