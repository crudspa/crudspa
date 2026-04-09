create proc [EducationSchool].[ReadPartSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    readPart.Id as Id
    ,readPart.AssessmentId as AssessmentId
    ,readPart.Title as Title
    ,readPart.PassageInstructionsText as PassageInstructionsText
    ,readPart.PassageInstructionsAudioFileId as PassageInstructionsAudioFileId
    ,readPart.PreviewInstructionsText as PreviewInstructionsText
    ,readPart.PreviewInstructionsAudioFileId as PreviewInstructionsAudioFileId
    ,readPart.QuestionsInstructionsText as QuestionsInstructionsText
    ,readPart.QuestionsInstructionsAudioFileId as QuestionsInstructionsAudioFileId
    ,readPart.Ordinal as Ordinal
    ,passageInstructionsAudioFile.Id as PassageInstructionsAudioFileId
    ,passageInstructionsAudioFile.BlobId as PassageInstructionsAudioFileBlobId
    ,passageInstructionsAudioFile.Name as PassageInstructionsAudioFileName
    ,passageInstructionsAudioFile.Format as PassageInstructionsAudioFileFormat
    ,passageInstructionsAudioFile.OptimizedStatus as PassageInstructionsAudioFileOptimizedStatus
    ,passageInstructionsAudioFile.OptimizedBlobId as PassageInstructionsAudioFileOptimizedBlobId
    ,passageInstructionsAudioFile.OptimizedFormat as PassageInstructionsAudioFileOptimizedFormat
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
    ,(select count(1) from [Education].[ReadParagraph-Active] readParagraph where readParagraph.ReadPartId = readPart.Id) as ReadParagraphCount
    ,(select count(1) from [Education].[ReadQuestion-Active] readQuestion where readQuestion.ReadPartId = readPart.Id) as ReadQuestionCount
from [Education].[ReadPart-Active] readPart
    left join [Framework].[AudioFile-Active] passageInstructionsAudioFile on readPart.PassageInstructionsAudioFileId = passageInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on readPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on readPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
where readPart.AssessmentId = @AssessmentId