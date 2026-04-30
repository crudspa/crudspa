create proc [ContentDesign].[OptionsAnswerChoiceSelectForOptionsAnswer] (
     @OptionsAnswerId uniqueidentifier
) as

select
     choice.Id
    ,choice.OptionsAnswerId
    ,choice.Text
    ,choice.Ordinal
from [Content].[OptionsAnswerChoice-Active] choice
where choice.OptionsAnswerId = @OptionsAnswerId
order by choice.Ordinal