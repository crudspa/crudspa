create trigger [Education].[VocabPartTrigger] on [Education].[VocabPart]
    for update
as

insert [Education].[VocabPart] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssessmentId
    ,Title
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
    ,deleted.PreviewInstructionsText
    ,deleted.PreviewInstructionsAudioFileId
    ,deleted.QuestionsInstructionsText
    ,deleted.QuestionsInstructionsAudioFileId
    ,deleted.Ordinal
from deleted