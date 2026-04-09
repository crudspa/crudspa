create trigger [Education].[ListenPartTrigger] on [Education].[ListenPart]
    for update
as

insert [Education].[ListenPart] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssessmentId
    ,Title
    ,PassageAudioFileId
    ,PassageInstructionsText
    ,PassageInstructionsAudioFileId
    ,PreviewInstructionsText
    ,PreviewInstructionsAudioFileId
    ,QuestionsInstructionsText
    ,QuestionsInstructionsAudioFileId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AssessmentId
    ,deleted.Title
    ,deleted.PassageAudioFileId
    ,deleted.PassageInstructionsText
    ,deleted.PassageInstructionsAudioFileId
    ,deleted.PreviewInstructionsText
    ,deleted.PreviewInstructionsAudioFileId
    ,deleted.QuestionsInstructionsText
    ,deleted.QuestionsInstructionsAudioFileId
    ,deleted.Ordinal
from deleted