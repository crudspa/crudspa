create proc [EducationPublisher].[ListenPartSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     listenPart.Id
    ,listenPart.AssessmentId
    ,assessment.Name as AssessmentName
    ,listenPart.Title
    ,passageAudioFile.Id as PassageAudioFileId
    ,passageAudioFile.BlobId as PassageAudioFileBlobId
    ,passageAudioFile.Name as PassageAudioFileName
    ,passageAudioFile.Format as PassageAudioFileFormat
    ,passageAudioFile.OptimizedStatus as PassageAudioFileOptimizedStatus
    ,passageAudioFile.OptimizedBlobId as PassageAudioFileOptimizedBlobId
    ,passageAudioFile.OptimizedFormat as PassageAudioFileOptimizedFormat
    ,listenPart.PassageInstructionsText
    ,passageInstructionsAudioFile.Id as PassageInstructionsAudioFileId
    ,passageInstructionsAudioFile.BlobId as PassageInstructionsAudioFileBlobId
    ,passageInstructionsAudioFile.Name as PassageInstructionsAudioFileName
    ,passageInstructionsAudioFile.Format as PassageInstructionsAudioFileFormat
    ,passageInstructionsAudioFile.OptimizedStatus as PassageInstructionsAudioFileOptimizedStatus
    ,passageInstructionsAudioFile.OptimizedBlobId as PassageInstructionsAudioFileOptimizedBlobId
    ,passageInstructionsAudioFile.OptimizedFormat as PassageInstructionsAudioFileOptimizedFormat
    ,listenPart.PreviewInstructionsText
    ,previewInstructionsAudioFile.Id as PreviewInstructionsAudioFileId
    ,previewInstructionsAudioFile.BlobId as PreviewInstructionsAudioFileBlobId
    ,previewInstructionsAudioFile.Name as PreviewInstructionsAudioFileName
    ,previewInstructionsAudioFile.Format as PreviewInstructionsAudioFileFormat
    ,previewInstructionsAudioFile.OptimizedStatus as PreviewInstructionsAudioFileOptimizedStatus
    ,previewInstructionsAudioFile.OptimizedBlobId as PreviewInstructionsAudioFileOptimizedBlobId
    ,previewInstructionsAudioFile.OptimizedFormat as PreviewInstructionsAudioFileOptimizedFormat
    ,listenPart.QuestionsInstructionsText
    ,questionsInstructionsAudioFile.Id as QuestionsInstructionsAudioFileId
    ,questionsInstructionsAudioFile.BlobId as QuestionsInstructionsAudioFileBlobId
    ,questionsInstructionsAudioFile.Name as QuestionsInstructionsAudioFileName
    ,questionsInstructionsAudioFile.Format as QuestionsInstructionsAudioFileFormat
    ,questionsInstructionsAudioFile.OptimizedStatus as QuestionsInstructionsAudioFileOptimizedStatus
    ,questionsInstructionsAudioFile.OptimizedBlobId as QuestionsInstructionsAudioFileOptimizedBlobId
    ,questionsInstructionsAudioFile.OptimizedFormat as QuestionsInstructionsAudioFileOptimizedFormat
    ,listenPart.Ordinal
    ,(select count(1) from [Education].[ListenQuestion-Active] where ListenPartId = listenPart.Id) as ListenQuestionCount
from [Education].[ListenPart-Active] listenPart
    inner join [Education].[Assessment-Active] assessment on listenPart.AssessmentId = assessment.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    inner join [Framework].[AudioFile-Active] passageAudioFile on listenPart.PassageAudioFileId = passageAudioFile.Id
    left join [Framework].[AudioFile-Active] passageInstructionsAudioFile on listenPart.PassageInstructionsAudioFileId = passageInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on listenPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on listenPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
where listenPart.Id = @Id
    and organization.Id = @organizationId