create trigger [Education].[ListenChoiceTrigger] on [Education].[ListenChoice]
    for update
as

insert [Education].[ListenChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ListenQuestionId
    ,Text
    ,IsCorrect
    ,ImageFileId
    ,AudioFileId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ListenQuestionId
    ,deleted.Text
    ,deleted.IsCorrect
    ,deleted.ImageFileId
    ,deleted.AudioFileId
    ,deleted.Ordinal
from deleted