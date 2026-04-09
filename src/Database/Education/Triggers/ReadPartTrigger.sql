create trigger [Education].[ReadPartTrigger] on [Education].[ReadPart]
    for update
as

insert [Education].[ReadPart] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssessmentId
    ,Title
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
    ,deleted.PassageInstructionsText
    ,deleted.PassageInstructionsAudioFileId
    ,deleted.PreviewInstructionsText
    ,deleted.PreviewInstructionsAudioFileId
    ,deleted.QuestionsInstructionsText
    ,deleted.QuestionsInstructionsAudioFileId
    ,deleted.Ordinal
from deleted