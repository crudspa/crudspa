create view [Content].[ScaleAnswer-Active] as

select scaleAnswer.Id as Id
    ,scaleAnswer.QuestionId as QuestionId
    ,scaleAnswer.Kind as Kind
    ,scaleAnswer.RatingKind as RatingKind
    ,scaleAnswer.LikertKind as LikertKind
    ,scaleAnswer.RatingMin as RatingMin
    ,scaleAnswer.RatingMax as RatingMax
    ,scaleAnswer.Ordering as Ordering
from [Content].[ScaleAnswer] scaleAnswer
where 1=1
    and scaleAnswer.IsDeleted = 0
    and scaleAnswer.VersionOf = scaleAnswer.Id