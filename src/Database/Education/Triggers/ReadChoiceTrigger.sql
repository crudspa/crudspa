create trigger [Education].[ReadChoiceTrigger] on [Education].[ReadChoice]
    for update
as

insert [Education].[ReadChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ReadQuestionId
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
    ,deleted.ReadQuestionId
    ,deleted.Text
    ,deleted.IsCorrect
    ,deleted.ImageFileId
    ,deleted.AudioFileId
    ,deleted.Ordinal
from deleted