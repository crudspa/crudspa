create proc [EducationSchool].[ListenPartSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    listenPart.Id as Id
    ,listenPart.AssessmentId as AssessmentId
    ,listenPart.Title as Title
    ,listenPart.PassageAudioFileId as PassageAudioFileId
    ,listenPart.PassageInstructionsText as PassageInstructionsText
    ,listenPart.PassageInstructionsAudioFileId as PassageInstructionsAudioFileId
    ,listenPart.PreviewInstructionsText as PreviewInstructionsText
    ,listenPart.PreviewInstructionsAudioFileId as PreviewInstructionsAudioFileId
    ,listenPart.QuestionsInstructionsText as QuestionsInstructionsText
    ,listenPart.QuestionsInstructionsAudioFileId as QuestionsInstructionsAudioFileId
    ,listenPart.Ordinal as Ordinal
    ,passageAudioFile.Id as PassageAudioFileId
    ,passageAudioFile.BlobId as PassageAudioFileBlobId
    ,passageAudioFile.Name as PassageAudioFileName
    ,passageAudioFile.Format as PassageAudioFileFormat
    ,passageAudioFile.OptimizedStatus as PassageAudioFileOptimizedStatus
    ,passageAudioFile.OptimizedBlobId as PassageAudioFileOptimizedBlobId
    ,passageAudioFile.OptimizedFormat as PassageAudioFileOptimizedFormat
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
    ,(select count(1) from [Education].[ListenQuestion-Active] listenQuestion where listenQuestion.ListenPartId = listenPart.Id) as ListenQuestionCount
from [Education].[ListenPart-Active] listenPart
    inner join [Framework].[AudioFile-Active] passageAudioFile on listenPart.PassageAudioFileId = passageAudioFile.Id
    left join [Framework].[AudioFile-Active] passageInstructionsAudioFile on listenPart.PassageInstructionsAudioFileId = passageInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on listenPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on listenPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
where listenPart.AssessmentId = @AssessmentId