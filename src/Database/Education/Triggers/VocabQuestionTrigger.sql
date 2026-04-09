create trigger [Education].[VocabQuestionTrigger] on [Education].[VocabQuestion]
    for update
as

insert [Education].[VocabQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,VocabPartId
    ,Word
    ,AudioFileId
    ,IsPreview
    ,PageBreakBefore
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.VocabPartId
    ,deleted.Word
    ,deleted.AudioFileId
    ,deleted.IsPreview
    ,deleted.PageBreakBefore
    ,deleted.Ordinal
from deleted