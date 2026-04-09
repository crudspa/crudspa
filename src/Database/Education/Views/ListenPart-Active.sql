create view [Education].[ListenPart-Active] as

select listenPart.Id as Id
    ,listenPart.AssessmentId as AssessmentId
    ,listenPart.Title as Title
    ,listenPart.PassageAudioFileId as PassageAudioFileId
    ,listenPart.PassageInstructionsText as PassageInstructionsText
    ,listenPart.PassageInstructionsAudioFileId as PassageInstructionsAudioFileId
    ,listenPart.PreviewInstructionsText as PreviewInstructionsText
    ,listenPart.PreviewInstructionsAudioFileId as PreviewInstructionsAudioFileId
    ,listenPart.QuestionsInstructionsText as QuestionsInstructionsText
    ,listenPart.QuestionsInstructionsAudioFileId as QuestionsInstructionsAudioFileId
    ,listenPart.Ordinal as Ordinal
from [Education].[ListenPart] listenPart
where 1=1
    and listenPart.IsDeleted = 0
    and listenPart.VersionOf = listenPart.Id