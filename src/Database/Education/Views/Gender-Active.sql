create view [Education].[Gender-Active] as

select gender.Id as Id
    ,gender.Name as Name
    ,gender.EnglishSubjectivePronoun as EnglishSubjectivePronoun
    ,gender.SpanishSubjectivePronoun as SpanishSubjectivePronoun
    ,gender.EnglishObjectivePronoun as EnglishObjectivePronoun
    ,gender.SpanishObjectivePronoun as SpanishObjectivePronoun
    ,gender.EnglishPossessivePronoun as EnglishPossessivePronoun
    ,gender.SpanishPossessivePronoun as SpanishPossessivePronoun
    ,gender.EnglishPossessiveAdjective as EnglishPossessiveAdjective
    ,gender.SpanishPossessiveAdjective as SpanishPossessiveAdjective
from [Education].[Gender] gender
where 1=1
    and gender.IsDeleted = 0