create view [Education].[ReadQuestion-Active] as

select readQuestion.Id as Id
    ,readQuestion.ReadPartId as ReadPartId
    ,readQuestion.Text as Text
    ,readQuestion.AudioFileId as AudioFileId
    ,readQuestion.IsPreview as IsPreview
    ,readQuestion.PageBreakBefore as PageBreakBefore
    ,readQuestion.HasCorrectChoice as HasCorrectChoice
    ,readQuestion.RequireTextInput as RequireTextInput
    ,readQuestion.TypeId as TypeId
    ,readQuestion.CategoryId as CategoryId
    ,readQuestion.ImageFileId as ImageFileId
    ,readQuestion.Ordinal as Ordinal
from [Education].[ReadQuestion] readQuestion
where 1=1
    and readQuestion.IsDeleted = 0
    and readQuestion.VersionOf = readQuestion.Id