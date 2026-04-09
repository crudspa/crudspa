create view [Education].[VocabQuestion-Active] as

select vocabQuestion.Id as Id
    ,vocabQuestion.VocabPartId as VocabPartId
    ,vocabQuestion.Word as Word
    ,vocabQuestion.AudioFileId as AudioFileId
    ,vocabQuestion.IsPreview as IsPreview
    ,vocabQuestion.PageBreakBefore as PageBreakBefore
    ,vocabQuestion.Ordinal as Ordinal
from [Education].[VocabQuestion] vocabQuestion
where 1=1
    and vocabQuestion.IsDeleted = 0
    and vocabQuestion.VersionOf = vocabQuestion.Id