create proc [EducationSchool].[VocabChoiceSelectionSelectForAssessmentAssignment] (
     @AssessmentAssignmentId uniqueidentifier
) as

select
    vocabChoiceSelection.Id
    ,vocabChoiceSelection.AssignmentId
    ,vocabChoiceSelection.ChoiceId
    ,vocabChoiceSelection.Made
    ,choice.VocabQuestionId
    ,choice.Word
from [Education].[VocabChoiceSelection-Active] vocabChoiceSelection
    inner join [Education].[VocabChoice-Active] choice on vocabChoiceSelection.ChoiceId = choice.Id
where vocabChoiceSelection.AssignmentId = @AssessmentAssignmentId
order by vocabChoiceSelection.Made desc