create view [Content].[TextAnswer-Active] as

select textAnswer.Id as Id
    ,textAnswer.QuestionId as QuestionId
    ,textAnswer.Kind as Kind
    ,textAnswer.Label as Label
    ,textAnswer.Placeholder as Placeholder
from [Content].[TextAnswer] textAnswer
where 1=1
    and textAnswer.IsDeleted = 0
    and textAnswer.VersionOf = textAnswer.Id