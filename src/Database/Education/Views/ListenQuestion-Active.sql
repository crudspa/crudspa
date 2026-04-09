create view [Education].[ListenQuestion-Active] as

select listenQuestion.Id as Id
    ,listenQuestion.ListenPartId as ListenPartId
    ,listenQuestion.Text as Text
    ,listenQuestion.AudioFileId as AudioFileId
    ,listenQuestion.IsPreview as IsPreview
    ,listenQuestion.PageBreakBefore as PageBreakBefore
    ,listenQuestion.HasCorrectChoice as HasCorrectChoice
    ,listenQuestion.RequireTextInput as RequireTextInput
    ,listenQuestion.CategoryId as CategoryId
    ,listenQuestion.ImageFileId as ImageFileId
    ,listenQuestion.TypeId as TypeId
    ,listenQuestion.Ordinal as Ordinal
from [Education].[ListenQuestion] listenQuestion
where 1=1
    and listenQuestion.IsDeleted = 0
    and listenQuestion.VersionOf = listenQuestion.Id