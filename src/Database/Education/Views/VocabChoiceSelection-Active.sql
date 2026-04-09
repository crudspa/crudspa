create view [Education].[VocabChoiceSelection-Active] as

select vocabChoiceSelection.Id as Id
    ,vocabChoiceSelection.AssignmentId as AssignmentId
    ,vocabChoiceSelection.ChoiceId as ChoiceId
    ,vocabChoiceSelection.Made as Made
from [Education].[VocabChoiceSelection] vocabChoiceSelection
where 1=1
    and vocabChoiceSelection.IsDeleted = 0