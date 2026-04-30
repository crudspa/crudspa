create view [Content].[FileAnswer-Active] as

select fileAnswer.Id as Id
    ,fileAnswer.QuestionId as QuestionId
    ,fileAnswer.Kind as Kind
from [Content].[FileAnswer] fileAnswer
where 1=1
    and fileAnswer.IsDeleted = 0
    and fileAnswer.VersionOf = fileAnswer.Id