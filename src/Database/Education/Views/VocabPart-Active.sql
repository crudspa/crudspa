create view [Education].[VocabPart-Active] as

select vocabPart.Id as Id
    ,vocabPart.AssessmentId as AssessmentId
    ,vocabPart.Title as Title
    ,vocabPart.PreviewInstructionsText as PreviewInstructionsText
    ,vocabPart.PreviewInstructionsAudioFileId as PreviewInstructionsAudioFileId
    ,vocabPart.QuestionsInstructionsText as QuestionsInstructionsText
    ,vocabPart.QuestionsInstructionsAudioFileId as QuestionsInstructionsAudioFileId
    ,vocabPart.Ordinal as Ordinal
from [Education].[VocabPart] vocabPart
where 1=1
    and vocabPart.IsDeleted = 0
    and vocabPart.VersionOf = vocabPart.Id