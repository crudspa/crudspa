create view [Content].[OptionsAnswerChoice-Active] as

select optionsAnswerChoice.Id as Id
    ,optionsAnswerChoice.OptionsAnswerId as OptionsAnswerId
    ,optionsAnswerChoice.Text as Text
    ,optionsAnswerChoice.Ordinal as Ordinal
from [Content].[OptionsAnswerChoice] optionsAnswerChoice
where 1=1
    and optionsAnswerChoice.IsDeleted = 0
    and optionsAnswerChoice.VersionOf = optionsAnswerChoice.Id