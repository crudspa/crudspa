create view [Content].[NumberAnswer-Active] as

select numberAnswer.Id as Id
    ,numberAnswer.QuestionId as QuestionId
    ,numberAnswer.Kind as Kind
    ,numberAnswer.Label as Label
    ,numberAnswer.IntegerMin as IntegerMin
    ,numberAnswer.IntegerMax as IntegerMax
    ,numberAnswer.DecimalMin as DecimalMin
    ,numberAnswer.DecimalMax as DecimalMax
    ,numberAnswer.CurrencyMin as CurrencyMin
    ,numberAnswer.CurrencyMax as CurrencyMax
from [Content].[NumberAnswer] numberAnswer
where 1=1
    and numberAnswer.IsDeleted = 0
    and numberAnswer.VersionOf = numberAnswer.Id