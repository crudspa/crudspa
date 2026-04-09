create proc [EducationPublisher].[ListenQuestionSelect] (
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
     listenQuestion.Id
    ,listenQuestion.ListenPartId
    ,listenPart.Title as ListenPartTitle
    ,listenQuestion.Text
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,listenQuestion.IsPreview
    ,listenQuestion.PageBreakBefore
    ,listenQuestion.HasCorrectChoice
    ,listenQuestion.RequireTextInput
    ,listenQuestion.CategoryId
    ,category.Name as CategoryName
    ,listenQuestion.TypeId
    ,type.Name as TypeName
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,listenQuestion.Ordinal
from [Education].[ListenQuestion-Active] listenQuestion
    left join [Framework].[AudioFile-Active] audioFile on listenQuestion.AudioFileId = audioFile.Id
    inner join [Education].[ListenQuestionCategory-Active] category on listenQuestion.CategoryId = category.Id
    left join [Framework].[ImageFile-Active] imageFile on listenQuestion.ImageFileId = imageFile.Id
    inner join [Education].[ListenPart-Active] listenPart on listenQuestion.ListenPartId = listenPart.Id
    inner join [Education].[Assessment-Active] assessment on listenPart.AssessmentId = assessment.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    inner join [Education].[ReadQuestionType-Active] type on listenQuestion.TypeId = type.Id
where listenQuestion.Id = @Id
    and organization.Id = @organizationId

select
     listenChoice.Id
    ,listenChoice.ListenQuestionId
    ,listenQuestion.Text as ListenQuestionText
    ,listenChoice.Text
    ,listenChoice.IsCorrect
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
    ,listenChoice.Ordinal
from [Education].[ListenChoice-Active] listenChoice
    left join [Framework].[AudioFile-Active] audioFile on listenChoice.AudioFileId = audioFile.Id
    left join [Framework].[ImageFile-Active] imageFile on listenChoice.ImageFileId = imageFile.Id
    inner join [Education].[ListenQuestion-Active] listenQuestion on listenChoice.ListenQuestionId = listenQuestion.Id
where listenChoice.ListenQuestionId = @Id