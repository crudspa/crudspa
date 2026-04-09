create proc [EducationSchool].[VocabChoiceSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    vocabChoice.Id
    ,vocabChoice.VocabQuestionId
    ,vocabChoice.Word
    ,vocabChoice.IsCorrect
    ,vocabChoice.Ordinal
from [Education].[VocabChoice-Active] vocabChoice
    inner join [Education].[VocabQuestion] vocabQuestion on vocabChoice.VocabQuestionId = vocabQuestion.Id
    inner join [Education].[VocabPart] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id
    inner join [Education].[Assessment] assessment on vocabPart.AssessmentId = assessment.Id
where assessment.Id = @AssessmentId