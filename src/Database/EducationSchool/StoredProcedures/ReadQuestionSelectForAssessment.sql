create proc [EducationSchool].[ReadQuestionSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    readQuestion.Id as Id
    ,readQuestion.ReadPartId as ReadPartId
    ,readQuestion.Text as Text
    ,readQuestion.IsPreview as IsPreview
    ,readQuestion.PageBreakBefore as PageBreakBefore
    ,readQuestion.HasCorrectChoice as HasCorrectChoice
    ,readQuestion.RequireTextInput as RequireTextInput
    ,readQuestion.Ordinal as Ordinal
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
from [Education].[ReadQuestion-Active] readQuestion
    inner join [Education].[ReadPart-Active] readPart on readQuestion.ReadPartId = readPart.Id
    inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
    left join [Framework].[AudioFile-Active] audioFile on readQuestion.AudioFileId = audioFile.Id
where assessment.Id = @AssessmentId