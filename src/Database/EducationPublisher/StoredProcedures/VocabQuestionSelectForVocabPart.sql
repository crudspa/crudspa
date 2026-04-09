create proc [EducationPublisher].[VocabQuestionSelectForVocabPart] (
     @SessionId uniqueidentifier
    ,@VocabPartId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     vocabQuestion.Id
    ,vocabQuestion.VocabPartId
    ,vocabPart.Title as VocabPartTitle
    ,vocabQuestion.Word
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,vocabQuestion.IsPreview
    ,vocabQuestion.PageBreakBefore
    ,vocabQuestion.Ordinal
from [Education].[VocabQuestion-Active] vocabQuestion
    left join [Framework].[AudioFile-Active] audioFile on vocabQuestion.AudioFileId = audioFile.Id
    inner join [Education].[VocabPart-Active] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id
    inner join [Education].[Assessment-Active] assessment on vocabPart.AssessmentId = assessment.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
where vocabQuestion.VocabPartId = @VocabPartId
    and organization.Id = @organizationId

select
     vocabChoice.Id
    ,vocabChoice.VocabQuestionId
    ,vocabQuestion.Word as VocabQuestionWord
    ,vocabChoice.Word
    ,vocabChoice.IsCorrect
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,vocabChoice.Ordinal
from [Education].[VocabChoice-Active] vocabChoice
    left join [Framework].[AudioFile-Active] audioFile on vocabChoice.AudioFileId = audioFile.Id
    inner join [Education].[VocabQuestion-Active] vocabQuestion on vocabChoice.VocabQuestionId = vocabQuestion.Id
where vocabQuestion.VocabPartId = @VocabPartId