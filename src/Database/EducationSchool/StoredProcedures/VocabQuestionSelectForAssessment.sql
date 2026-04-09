create proc [EducationSchool].[VocabQuestionSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    vocabQuestion.Id as Id
    ,vocabQuestion.VocabPartId as VocabPartId
    ,vocabQuestion.Word as Word
    ,vocabQuestion.AudioFileId as AudioFileId
    ,vocabQuestion.IsPreview as IsPreview
    ,vocabQuestion.PageBreakBefore as PageBreakBefore
    ,vocabQuestion.Ordinal as Ordinal
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
from [Education].[VocabQuestion-Active] vocabQuestion
    inner join [Education].[VocabPart-Active] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id
    inner join [Education].[Assessment-Active] assessment on vocabPart.AssessmentId = assessment.Id
    left join [Framework].[AudioFile-Active] audioFile on vocabQuestion.AudioFileId = audioFile.Id
where assessment.Id = @AssessmentId