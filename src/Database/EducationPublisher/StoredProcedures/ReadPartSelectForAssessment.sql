create proc [EducationPublisher].[ReadPartSelectForAssessment] (
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
     readPart.Id
    ,readPart.AssessmentId
    ,readPart.Title
    ,readPart.PassageInstructionsText
    ,passageInstructionsAudioFile.Id as PassageInstructionsAudioFileId
    ,passageInstructionsAudioFile.BlobId as PassageInstructionsAudioFileBlobId
    ,passageInstructionsAudioFile.Name as PassageInstructionsAudioFileName
    ,passageInstructionsAudioFile.Format as PassageInstructionsAudioFileFormat
    ,passageInstructionsAudioFile.OptimizedStatus as PassageInstructionsAudioFileOptimizedStatus
    ,passageInstructionsAudioFile.OptimizedBlobId as PassageInstructionsAudioFileOptimizedBlobId
    ,passageInstructionsAudioFile.OptimizedFormat as PassageInstructionsAudioFileOptimizedFormat
    ,readPart.PreviewInstructionsText
    ,previewInstructionsAudioFile.Id as PreviewInstructionsAudioFileId
    ,previewInstructionsAudioFile.BlobId as PreviewInstructionsAudioFileBlobId
    ,previewInstructionsAudioFile.Name as PreviewInstructionsAudioFileName
    ,previewInstructionsAudioFile.Format as PreviewInstructionsAudioFileFormat
    ,previewInstructionsAudioFile.OptimizedStatus as PreviewInstructionsAudioFileOptimizedStatus
    ,previewInstructionsAudioFile.OptimizedBlobId as PreviewInstructionsAudioFileOptimizedBlobId
    ,previewInstructionsAudioFile.OptimizedFormat as PreviewInstructionsAudioFileOptimizedFormat
    ,readPart.QuestionsInstructionsText
    ,questionsInstructionsAudioFile.Id as QuestionsInstructionsAudioFileId
    ,questionsInstructionsAudioFile.BlobId as QuestionsInstructionsAudioFileBlobId
    ,questionsInstructionsAudioFile.Name as QuestionsInstructionsAudioFileName
    ,questionsInstructionsAudioFile.Format as QuestionsInstructionsAudioFileFormat
    ,questionsInstructionsAudioFile.OptimizedStatus as QuestionsInstructionsAudioFileOptimizedStatus
    ,questionsInstructionsAudioFile.OptimizedBlobId as QuestionsInstructionsAudioFileOptimizedBlobId
    ,questionsInstructionsAudioFile.OptimizedFormat as QuestionsInstructionsAudioFileOptimizedFormat
    ,readPart.Ordinal
    ,(select count(1) from [Education].[ReadParagraph-Active] where ReadPartId = readPart.Id) as ReadParagraphCount
    ,(select count(1) from [Education].[ReadQuestion-Active] where ReadPartId = readPart.Id) as ReadQuestionCount
from [Education].[ReadPart-Active] readPart
    inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    left join [Framework].[AudioFile-Active] passageInstructionsAudioFile on readPart.PassageInstructionsAudioFileId = passageInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on readPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on readPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
where readPart.AssessmentId = @AssessmentId
    and organization.Id = @organizationId