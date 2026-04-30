create view [Content].[BooleanAnswer-Active] as

select booleanAnswer.Id as Id
    ,booleanAnswer.QuestionId as QuestionId
    ,booleanAnswer.Kind as Kind
    ,booleanAnswer.[Default] as [Default]
    ,booleanAnswer.Orientation as Orientation
    ,booleanAnswer.TrueLabel as TrueLabel
    ,booleanAnswer.FalseLabel as FalseLabel
from [Content].[BooleanAnswer] booleanAnswer
where 1=1
    and booleanAnswer.IsDeleted = 0
    and booleanAnswer.VersionOf = booleanAnswer.Id