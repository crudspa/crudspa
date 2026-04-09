create proc [EducationPublisher].[VocabPartSelectForAssessment] (
     @SessionId uniqueidentifier
    ,@AssessmentId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     vocabPart.Id
    ,vocabPart.AssessmentId
    ,assessment.Name as AssessmentName
    ,vocabPart.Title
    ,vocabPart.PreviewInstructionsText
    ,previewInstructionsAudioFile.Id as PreviewInstructionsAudioFileId
    ,previewInstructionsAudioFile.BlobId as PreviewInstructionsAudioFileBlobId
    ,previewInstructionsAudioFile.Name as PreviewInstructionsAudioFileName
    ,previewInstructionsAudioFile.Format as PreviewInstructionsAudioFileFormat
    ,previewInstructionsAudioFile.OptimizedStatus as PreviewInstructionsAudioFileOptimizedStatus
    ,previewInstructionsAudioFile.OptimizedBlobId as PreviewInstructionsAudioFileOptimizedBlobId
    ,previewInstructionsAudioFile.OptimizedFormat as PreviewInstructionsAudioFileOptimizedFormat
    ,vocabPart.QuestionsInstructionsText
    ,questionsInstructionsAudioFile.Id as QuestionsInstructionsAudioFileId
    ,questionsInstructionsAudioFile.BlobId as QuestionsInstructionsAudioFileBlobId
    ,questionsInstructionsAudioFile.Name as QuestionsInstructionsAudioFileName
    ,questionsInstructionsAudioFile.Format as QuestionsInstructionsAudioFileFormat
    ,questionsInstructionsAudioFile.OptimizedStatus as QuestionsInstructionsAudioFileOptimizedStatus
    ,questionsInstructionsAudioFile.OptimizedBlobId as QuestionsInstructionsAudioFileOptimizedBlobId
    ,questionsInstructionsAudioFile.OptimizedFormat as QuestionsInstructionsAudioFileOptimizedFormat
    ,vocabPart.Ordinal
    ,(select count(1) from [Education].[VocabQuestion-Active] where VocabPartId = vocabPart.Id) as VocabQuestionCount
from [Education].[VocabPart-Active] vocabPart
    inner join [Education].[Assessment-Active] assessment on vocabPart.AssessmentId = assessment.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on vocabPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on vocabPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
where vocabPart.AssessmentId = @AssessmentId
    and organization.Id = @organizationId