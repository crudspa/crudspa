create view [Content].[ContactAnswer-Active] as

select contactAnswer.Id as Id
    ,contactAnswer.QuestionId as QuestionId
    ,contactAnswer.Kind as Kind
    ,contactAnswer.Label as Label
from [Content].[ContactAnswer] contactAnswer
where 1=1
    and contactAnswer.IsDeleted = 0
    and contactAnswer.VersionOf = contactAnswer.Id