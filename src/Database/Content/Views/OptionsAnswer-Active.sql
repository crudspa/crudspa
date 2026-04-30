create view [Content].[OptionsAnswer-Active] as

select optionsAnswer.Id as Id
    ,optionsAnswer.QuestionId as QuestionId
    ,optionsAnswer.Kind as Kind
    ,optionsAnswer.Orientation as Orientation
    ,optionsAnswer.AllowOther as AllowOther
    ,optionsAnswer.OtherLabel as OtherLabel
    ,optionsAnswer.MinSelections as MinSelections
    ,optionsAnswer.MaxSelections as MaxSelections
    ,optionsAnswer.Ordering as Ordering
from [Content].[OptionsAnswer] optionsAnswer
where 1=1
    and optionsAnswer.IsDeleted = 0
    and optionsAnswer.VersionOf = optionsAnswer.Id