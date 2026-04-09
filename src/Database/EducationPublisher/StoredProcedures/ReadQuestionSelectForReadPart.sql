create proc [EducationPublisher].[ReadQuestionSelectForReadPart] (
     @SessionId uniqueidentifier
    ,@ReadPartId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     readQuestion.Id
    ,readQuestion.ReadPartId
    ,readPart.Title as ReadPartTitle
    ,readQuestion.Text
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,readQuestion.IsPreview
    ,readQuestion.PageBreakBefore
    ,readQuestion.HasCorrectChoice
    ,readQuestion.RequireTextInput
    ,readQuestion.CategoryId
    ,category.Name as CategoryName
    ,readQuestion.TypeId
    ,type.Name as TypeName
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,readQuestion.Ordinal
from [Education].[ReadQuestion-Active] readQuestion
    left join [Framework].[AudioFile-Active] audioFile on readQuestion.AudioFileId = audioFile.Id
    inner join [Education].[ReadQuestionCategory-Active] category on readQuestion.CategoryId = category.Id
    left join [Framework].[ImageFile-Active] imageFile on readQuestion.ImageFileId = imageFile.Id
    inner join [Education].[ReadPart-Active] readPart on readQuestion.ReadPartId = readPart.Id
    inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    inner join [Education].[ReadQuestionType-Active] type on readQuestion.TypeId = type.Id
where readQuestion.ReadPartId = @ReadPartId
    and organization.Id = @organizationId

select
     readChoice.Id
    ,readChoice.ReadQuestionId
    ,readQuestion.Text as ReadQuestionText
    ,readChoice.Text
    ,readChoice.IsCorrect
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,readChoice.Ordinal
from [Education].[ReadChoice-Active] readChoice
    left join [Framework].[AudioFile-Active] audioFile on readChoice.AudioFileId = audioFile.Id
    left join [Framework].[ImageFile-Active] imageFile on readChoice.ImageFileId = imageFile.Id
    inner join [Education].[ReadQuestion-Active] readQuestion on readChoice.ReadQuestionId = readQuestion.Id
where readQuestion.ReadPartId = @ReadPartId