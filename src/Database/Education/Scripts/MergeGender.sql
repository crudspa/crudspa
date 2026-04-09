/*
merge into [Education].[Gender] as Target
using ( values
-- todo: Add values
) as Source
    (Id, Name, EnglishSubjectivePronoun, SpanishSubjectivePronoun, EnglishObjectivePronoun, SpanishObjectivePronoun, EnglishPossessivePronoun, SpanishPossessivePronoun, EnglishPossessiveAdjective, SpanishPossessiveAdjective)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.EnglishSubjectivePronoun = Source.EnglishSubjectivePronoun
    ,Target.SpanishSubjectivePronoun = Source.SpanishSubjectivePronoun
    ,Target.EnglishObjectivePronoun = Source.EnglishObjectivePronoun
    ,Target.SpanishObjectivePronoun = Source.SpanishObjectivePronoun
    ,Target.EnglishPossessivePronoun = Source.EnglishPossessivePronoun
    ,Target.SpanishPossessivePronoun = Source.SpanishPossessivePronoun
    ,Target.EnglishPossessiveAdjective = Source.EnglishPossessiveAdjective
    ,Target.SpanishPossessiveAdjective = Source.SpanishPossessiveAdjective

when not matched by target then
insert (Id, Name, EnglishSubjectivePronoun, SpanishSubjectivePronoun, EnglishObjectivePronoun, SpanishObjectivePronoun, EnglishPossessivePronoun, SpanishPossessivePronoun, EnglishPossessiveAdjective, SpanishPossessiveAdjective)
values (Id, Name, EnglishSubjectivePronoun, SpanishSubjectivePronoun, EnglishObjectivePronoun, SpanishObjectivePronoun, EnglishPossessivePronoun, SpanishPossessivePronoun, EnglishPossessiveAdjective, SpanishPossessiveAdjective)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/