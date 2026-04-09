create view [Education].[VocabChoice-Active] as

select vocabChoice.Id as Id
    ,vocabChoice.VocabQuestionId as VocabQuestionId
    ,vocabChoice.Word as Word
    ,vocabChoice.IsCorrect as IsCorrect
    ,vocabChoice.AudioFileId as AudioFileId
    ,vocabChoice.Ordinal as Ordinal
from [Education].[VocabChoice] vocabChoice
where 1=1
    and vocabChoice.IsDeleted = 0
    and vocabChoice.VersionOf = vocabChoice.Id