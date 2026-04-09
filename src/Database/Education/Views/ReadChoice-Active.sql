create view [Education].[ReadChoice-Active] as

select readChoice.Id as Id
    ,readChoice.ReadQuestionId as ReadQuestionId
    ,readChoice.Text as Text
    ,readChoice.IsCorrect as IsCorrect
    ,readChoice.ImageFileId as ImageFileId
    ,readChoice.AudioFileId as AudioFileId
    ,readChoice.Ordinal as Ordinal
from [Education].[ReadChoice] readChoice
where 1=1
    and readChoice.IsDeleted = 0
    and readChoice.VersionOf = readChoice.Id