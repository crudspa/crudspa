create view [Education].[ListenChoice-Active] as

select listenChoice.Id as Id
    ,listenChoice.ListenQuestionId as ListenQuestionId
    ,listenChoice.Text as Text
    ,listenChoice.IsCorrect as IsCorrect
    ,listenChoice.ImageFileId as ImageFileId
    ,listenChoice.AudioFileId as AudioFileId
    ,listenChoice.Ordinal as Ordinal
from [Education].[ListenChoice] listenChoice
where 1=1
    and listenChoice.IsDeleted = 0
    and listenChoice.VersionOf = listenChoice.Id