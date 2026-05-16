create view [Content].[DateAnswer-Active] as

select dateAnswer.Id as Id
    ,dateAnswer.QuestionId as QuestionId
    ,dateAnswer.Kind as Kind
    ,dateAnswer.Label as Label
    ,dateAnswer.DateMin as DateMin
    ,dateAnswer.DateMax as DateMax
    ,dateAnswer.TimeMin as TimeMin
    ,dateAnswer.TimeMax as TimeMax
    ,dateAnswer.DateTimeMin as DateTimeMin
    ,dateAnswer.DateTimeMax as DateTimeMax
from [Content].[DateAnswer] dateAnswer
where 1=1
    and dateAnswer.IsDeleted = 0
    and dateAnswer.VersionOf = dateAnswer.Id