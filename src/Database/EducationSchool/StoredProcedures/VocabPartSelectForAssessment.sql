create proc [EducationSchool].[VocabPartSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    vocabPart.Id as Id
    ,vocabPart.AssessmentId as AssessmentId
    ,vocabPart.Title as Title
    ,vocabPart.PreviewInstructionsText as PreviewInstructionsText
    ,vocabPart.QuestionsInstructionsText as QuestionsInstructionsText
    ,vocabPart.Ordinal as Ordinal
    ,previewInstructionsAudioFile.Id as PreviewInstructionsAudioFileId
    ,previewInstructionsAudioFile.BlobId as PreviewInstructionsAudioFileBlobId
    ,previewInstructionsAudioFile.Name as PreviewInstructionsAudioFileName
    ,previewInstructionsAudioFile.Format as PreviewInstructionsAudioFileFormat
    ,previewInstructionsAudioFile.OptimizedStatus as PreviewInstructionsAudioFileOptimizedStatus
    ,previewInstructionsAudioFile.OptimizedBlobId as PreviewInstructionsAudioFileOptimizedBlobId
    ,previewInstructionsAudioFile.OptimizedFormat as PreviewInstructionsAudioFileOptimizedFormat
    ,questionsInstructionsAudioFile.Id as QuestionsInstructionsAudioFileId
    ,questionsInstructionsAudioFile.BlobId as QuestionsInstructionsAudioFileBlobId
    ,questionsInstructionsAudioFile.Name as QuestionsInstructionsAudioFileName
    ,questionsInstructionsAudioFile.Format as QuestionsInstructionsAudioFileFormat
    ,questionsInstructionsAudioFile.OptimizedStatus as QuestionsInstructionsAudioFileOptimizedStatus
    ,questionsInstructionsAudioFile.OptimizedBlobId as QuestionsInstructionsAudioFileOptimizedBlobId
    ,questionsInstructionsAudioFile.OptimizedFormat as QuestionsInstructionsAudioFileOptimizedFormat
    ,(select count(1) from [Education].[VocabQuestion-Active] vocabQuestion where vocabQuestion.VocabPartId = vocabPart.Id) as VocabQuestionCount
from [Education].[VocabPart-Active] vocabPart
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on vocabPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on vocabPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
where vocabPart.AssessmentId = @AssessmentId