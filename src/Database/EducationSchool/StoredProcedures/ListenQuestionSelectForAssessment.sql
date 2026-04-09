create proc [EducationSchool].[ListenQuestionSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    listenQuestion.Id as Id
    ,listenQuestion.ListenPartId as ListenPartId
    ,listenQuestion.Text as Text
    ,listenQuestion.IsPreview as IsPreview
    ,listenQuestion.PageBreakBefore as PageBreakBefore
    ,listenQuestion.HasCorrectChoice as HasCorrectChoice
    ,listenQuestion.RequireTextInput as RequireTextInput
    ,listenQuestion.Ordinal as Ordinal
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
from [Education].[ListenQuestion-Active] listenQuestion
    inner join [Education].[ListenPart-Active] listenPart on listenQuestion.ListenPartId = listenPart.Id
    inner join [Education].[Assessment-Active] assessment on listenPart.AssessmentId = assessment.Id
    left join [Framework].[AudioFile-Active] audioFile on listenQuestion.AudioFileId = audioFile.Id
where assessment.Id = @AssessmentId