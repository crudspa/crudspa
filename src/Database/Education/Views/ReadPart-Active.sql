create view [Education].[ReadPart-Active] as

select readPart.Id as Id
    ,readPart.AssessmentId as AssessmentId
    ,readPart.Title as Title
    ,readPart.PassageInstructionsText as PassageInstructionsText
    ,readPart.PassageInstructionsAudioFileId as PassageInstructionsAudioFileId
    ,readPart.PreviewInstructionsText as PreviewInstructionsText
    ,readPart.PreviewInstructionsAudioFileId as PreviewInstructionsAudioFileId
    ,readPart.QuestionsInstructionsText as QuestionsInstructionsText
    ,readPart.QuestionsInstructionsAudioFileId as QuestionsInstructionsAudioFileId
    ,readPart.Ordinal as Ordinal
from [Education].[ReadPart] readPart
where 1=1
    and readPart.IsDeleted = 0
    and readPart.VersionOf = readPart.Id