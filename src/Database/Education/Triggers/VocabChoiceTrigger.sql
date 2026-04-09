create trigger [Education].[VocabChoiceTrigger] on [Education].[VocabChoice]
    for update
as

insert [Education].[VocabChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,VocabQuestionId
    ,Word
    ,IsCorrect
    ,AudioFileId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.VocabQuestionId
    ,deleted.Word
    ,deleted.IsCorrect
    ,deleted.AudioFileId
    ,deleted.Ordinal
from deleted