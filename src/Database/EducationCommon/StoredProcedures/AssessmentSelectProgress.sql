create proc [EducationCommon].[AssessmentSelectProgress] (
     @AssessmentAssignmentId uniqueidentifier
) as

select
    assessmentAssignment.Id as Id
    ,assessmentAssignment.AssessmentId
    ,assessmentAssignment.StudentId
    ,assessmentAssignment.Assigned
    ,assessmentAssignment.StartAfter
    ,assessmentAssignment.EndBefore
    ,assessmentAssignment.Started
    ,assessmentAssignment.Completed
    ,assessmentAssignment.Terminated
    ,assessment.Name as AssessmentName
    ,assessment.AvailableStart as AssessmentAvailableStart
    ,assessment.AvailableEnd as AssessmentAvailableEnd
    ,contact.FirstName as StudentFirstName
    ,contact.LastName as StudentLastName
    ,(select count(1) from [Education].[ListenPart-Active] listenPart where listenPart.AssessmentId = assessmentAssignment.AssessmentId) as ListenPartCount
    ,(select count(1) from [Education].[ListenQuestion-Active] listenQuestion inner join [Education].[ListenPart-Active] listenPart on listenQuestion.ListenPartId = listenPart.Id where listenPart.AssessmentId = assessmentAssignment.AssessmentId) as ListenQuestionCount
    ,(select count(1) from (
        select listenChoice.ListenQuestionId
        from [Education].[ListenChoiceSelection-Active] listenChoiceSelection
            inner join [Education].[ListenChoice-Active] listenChoice on listenChoiceSelection.ChoiceId = listenChoice.Id
        group by listenChoice.ListenQuestionId, listenChoiceSelection.AssignmentId
        having listenChoiceSelection.AssignmentId = assessmentAssignment.Id
    ) ListenQuestionsAnswered) as ListenChoiceSelectionCount
    ,(select count(1) from [Education].[ListenPartCompleted-Active] listenPartCompleted where listenPartCompleted.AssignmentId = assessmentAssignment.Id) as ListenPartCompletedCount
    ,(select count(1) from [Education].[ListenTextEntry-Active] listenTextEntry where listenTextEntry.AssignmentId = assessmentAssignment.Id) as ListenTextEntryCount
    ,(select count(1) from [Education].[ReadPart-Active] readPart where readPart.AssessmentId = assessmentAssignment.AssessmentId) as ReadPartCount
    ,(select count(1) from [Education].[ReadQuestion-Active] readQuestion inner join [Education].[ReadPart-Active] readPart on readQuestion.ReadPartId = readPart.Id where readPart.AssessmentId = assessmentAssignment.AssessmentId) as ReadQuestionCount
    ,(select count(1) from (
        select readChoice.ReadQuestionId
        from [Education].[ReadChoiceSelection-Active] readChoiceSelection
            inner join [Education].[ReadChoice-Active] readChoice on readChoiceSelection.ChoiceId = readChoice.Id
        group by readChoice.ReadQuestionId, readChoiceSelection.AssignmentId
        having readChoiceSelection.AssignmentId = assessmentAssignment.Id
    ) ReadQuestionsAnswered) as ReadChoiceSelectionCount
    ,(select count(1) from [Education].[ReadPartCompleted-Active] readPartCompleted where readPartCompleted.AssignmentId = assessmentAssignment.Id) as ReadPartCompletedCount
    ,(select count(1) from [Education].[ReadTextEntry-Active] readTextEntry where readTextEntry.AssignmentId = assessmentAssignment.Id) as ReadTextEntryCount
    ,(select count(1) from [Education].[VocabPart-Active] vocabPart where vocabPart.AssessmentId = assessmentAssignment.AssessmentId) as VocabPartCount
    ,(select count(1) from [Education].[VocabQuestion-Active] vocabQuestion inner join [Education].[VocabPart-Active] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id where vocabPart.AssessmentId = assessmentAssignment.AssessmentId) as VocabQuestionCount
    ,(select count(1) from (
        select vocabChoice.VocabQuestionId
        from [Education].[VocabChoiceSelection-Active] vocabChoiceSelection
            inner join [Education].[VocabChoice-Active] vocabChoice on vocabChoiceSelection.ChoiceId = vocabChoice.Id
        group by vocabChoice.VocabQuestionId, vocabChoiceSelection.AssignmentId
        having vocabChoiceSelection.AssignmentId = assessmentAssignment.Id
    ) VocabQuestionsAnswered) as VocabChoiceSelectionCount
    ,(select count(1) from [Education].[VocabPartCompleted-Active] vocabPartCompleted where vocabPartCompleted.AssignmentId = assessmentAssignment.Id) as VocabPartCompletedCount
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption

from [Education].[AssessmentAssignment-Active] assessmentAssignment
    inner join [Education].[Assessment-Active] assessment on assessmentAssignment.AssessmentId = assessment.Id
    inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    left join [Framework].[ImageFile-Active] image on assessment.ImageFileId = image.Id
where assessmentAssignment.Id = @AssessmentAssignmentId

select
    listenPartCompleted.Id
    ,listenPartCompleted.AssignmentId
    ,listenPartCompleted.ListenPartId
    ,listenPartCompleted.DeviceTimestamp
from [Education].[ListenPartCompleted-Active] listenPartCompleted
where listenPartCompleted.AssignmentId = @AssessmentAssignmentId

select
    readPartCompleted.Id
    ,readPartCompleted.AssignmentId
    ,readPartCompleted.ReadPartId
    ,readPartCompleted.DeviceTimestamp
from [Education].[ReadPartCompleted-Active] readPartCompleted
where readPartCompleted.AssignmentId = @AssessmentAssignmentId

select
    vocabPartCompleted.Id
    ,vocabPartCompleted.AssignmentId
    ,vocabPartCompleted.VocabPartId
    ,vocabPartCompleted.DeviceTimestamp
from [Education].[VocabPartCompleted-Active] vocabPartCompleted
where vocabPartCompleted.AssignmentId = @AssessmentAssignmentId

select
    listenPart.Id
    ,listenPart.AssessmentId
    ,listenPart.Title
    ,listenPart.PassageAudioFileId
    ,listenPart.PassageInstructionsText
    ,listenPart.PassageInstructionsAudioFileId
    ,listenPart.PreviewInstructionsText
    ,listenPart.PreviewInstructionsAudioFileId
    ,listenPart.QuestionsInstructionsText
    ,listenPart.QuestionsInstructionsAudioFileId
    ,listenPart.Ordinal
    ,passageAudioFile.Id as PassageAudioFileId
    ,passageAudioFile.BlobId as PassageAudioFileBlobId
    ,passageAudioFile.Name as PassageAudioFileName
    ,passageAudioFile.Format as PassageAudioFileFormat
    ,passageAudioFile.OptimizedStatus as PassageAudioFileOptimizedStatus
    ,passageAudioFile.OptimizedBlobId as PassageAudioFileOptimizedBlobId
    ,passageAudioFile.OptimizedFormat as PassageAudioFileOptimizedFormat
    ,passageInstructionsAudioFile.Id as PassageInstructionsAudioFileId
    ,passageInstructionsAudioFile.BlobId as PassageInstructionsAudioFileBlobId
    ,passageInstructionsAudioFile.Name as PassageInstructionsAudioFileName
    ,passageInstructionsAudioFile.Format as PassageInstructionsAudioFileFormat
    ,passageInstructionsAudioFile.OptimizedStatus as PassageInstructionsAudioFileOptimizedStatus
    ,passageInstructionsAudioFile.OptimizedBlobId as PassageInstructionsAudioFileOptimizedBlobId
    ,passageInstructionsAudioFile.OptimizedFormat as PassageInstructionsAudioFileOptimizedFormat
    ,previewInstructionsAudioFile.Id as PreviewInstructionsAudioFileId
    ,previewInstructionsAudioFile.BlobId as PreviewInstructionsAudioFileBlobId
    ,previewInstructionsAudioFile.Name as PreviewInstructionsAudioFileName
    ,previewInstructionsAudioFile.Format as PreviewInstructionsAudioFileFormat
    ,previewInstructionsAudioFile.OptimizedStatus as PreviewInstructionsAudioFileOptimizedStatus
    ,previewInstructionsAudioFile.OptimizedBlobId as PreviewInstructionsAudioFileOptimizedBlobId
    ,previewInstructionsAudioFile.OptimizedFormat as PreviewInstructionsAudioFileOptimizedFormat
    ,questionsInstructionsAudioFile.Id as QuestionsInstructionsAudioFileId
    ,questionsInstructionsAudioFile.BlobId as QuestionsInstructionsAudioFileBlobId
    ,questionsInstructionsAudioFile.Name as QuestionsInstructionsAudioFileName
    ,questionsInstructionsAudioFile.Format as QuestionsInstructionsAudioFileFormat
    ,questionsInstructionsAudioFile.OptimizedStatus as QuestionsInstructionsAudioFileOptimizedStatus
    ,questionsInstructionsAudioFile.OptimizedBlobId as QuestionsInstructionsAudioFileOptimizedBlobId
    ,questionsInstructionsAudioFile.OptimizedFormat as QuestionsInstructionsAudioFileOptimizedFormat
    ,(select count(1) from [Education].[ListenQuestion-Active] listenQuestion where listenQuestion.ListenPartId = listenPart.Id) as ListenQuestionCount
from [Education].[ListenPart-Active] listenPart
    inner join [Framework].[AudioFile-Active] passageAudioFile on listenPart.PassageAudioFileId = passageAudioFile.Id
    left join [Framework].[AudioFile-Active] passageInstructionsAudioFile on listenPart.PassageInstructionsAudioFileId = passageInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on listenPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on listenPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = listenPart.AssessmentId
where assessmentAssignment.Id = @AssessmentAssignmentId
order by listenPart.Ordinal

select
    readPart.Id
    ,readPart.AssessmentId
    ,readPart.Title
    ,readPart.PassageInstructionsText
    ,readPart.PassageInstructionsAudioFileId
    ,readPart.PreviewInstructionsText
    ,readPart.PreviewInstructionsAudioFileId
    ,readPart.QuestionsInstructionsText
    ,readPart.QuestionsInstructionsAudioFileId
    ,readPart.Ordinal
    ,passageInstructionsAudioFile.Id as PassageInstructionsAudioFileId
    ,passageInstructionsAudioFile.BlobId as PassageInstructionsAudioFileBlobId
    ,passageInstructionsAudioFile.Name as PassageInstructionsAudioFileName
    ,passageInstructionsAudioFile.Format as PassageInstructionsAudioFileFormat
    ,passageInstructionsAudioFile.OptimizedStatus as PassageInstructionsAudioFileOptimizedStatus
    ,passageInstructionsAudioFile.OptimizedBlobId as PassageInstructionsAudioFileOptimizedBlobId
    ,passageInstructionsAudioFile.OptimizedFormat as PassageInstructionsAudioFileOptimizedFormat
    ,previewInstructionsAudioFile.Id as PreviewInstructionsAudioFileId
    ,previewInstructionsAudioFile.BlobId as PreviewInstructionsAudioFileBlobId
    ,previewInstructionsAudioFile.Name as PreviewInstructionsAudioFileName
    ,previewInstructionsAudioFile.Format as PreviewInstructionsAudioFileFormat
    ,previewInstructionsAudioFile.OptimizedStatus as PreviewInstructionsAudioFileOptimizedStatus
    ,previewInstructionsAudioFile.OptimizedBlobId as PreviewInstructionsAudioFileOptimizedBlobId
    ,previewInstructionsAudioFile.OptimizedFormat as PreviewInstructionsAudioFileOptimizedFormat
    ,questionsInstructionsAudioFile.Id as QuestionsInstructionsAudioFileId
    ,questionsInstructionsAudioFile.BlobId as QuestionsInstructionsAudioFileBlobId
    ,questionsInstructionsAudioFile.Name as QuestionsInstructionsAudioFileName
    ,questionsInstructionsAudioFile.Format as QuestionsInstructionsAudioFileFormat
    ,questionsInstructionsAudioFile.OptimizedStatus as QuestionsInstructionsAudioFileOptimizedStatus
    ,questionsInstructionsAudioFile.OptimizedBlobId as QuestionsInstructionsAudioFileOptimizedBlobId
    ,questionsInstructionsAudioFile.OptimizedFormat as QuestionsInstructionsAudioFileOptimizedFormat
    ,(select count(1) from [Education].[ReadParagraph-Active] readParagraph where readParagraph.ReadPartId = readPart.Id) as ReadParagraphCount
    ,(select count(1) from [Education].[ReadQuestion-Active] readQuestion where readQuestion.ReadPartId = readPart.Id) as ReadQuestionCount
from [Education].[ReadPart-Active] readPart
    left join [Framework].[AudioFile-Active] passageInstructionsAudioFile on readPart.PassageInstructionsAudioFileId = passageInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on readPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on readPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = readPart.AssessmentId
where assessmentAssignment.Id = @AssessmentAssignmentId
order by readPart.Ordinal

select
    vocabPart.Id
    ,vocabPart.AssessmentId
    ,vocabPart.Title
    ,vocabPart.PreviewInstructionsText
    ,vocabPart.QuestionsInstructionsText
    ,vocabPart.Ordinal
    ,previewInstructionsAudioFile.Id as PreviewInstructionsAudioFileId
    ,previewInstructionsAudioFile.BlobId as PreviewInstructionsAudioFileBlobId
    ,previewInstructionsAudioFile.Name as PreviewInstructionsAudioFileName
    ,previewInstructionsAudioFile.Format as PreviewInstructionsAudioFileFormat
    ,previewInstructionsAudioFile.OptimizedStatus as PreviewInstructionsAudioFileOptimizedStatus
    ,previewInstructionsAudioFile.OptimizedBlobId as PreviewInstructionsAudioFileOptimizedBlobId
    ,previewInstructionsAudioFile.OptimizedFormat as PreviewInstructionsAudioFileOptimizedFormat
    ,questionsInstructionsAudioFile.Id as QuestionsInstructionsAudioFileId
    ,questionsInstructionsAudioFile.BlobId as QuestionsInstructionsAudioFileBlobId
    ,questionsInstructionsAudioFile.Name as QuestionsInstructionsAudioFileName
    ,questionsInstructionsAudioFile.Format as QuestionsInstructionsAudioFileFormat
    ,questionsInstructionsAudioFile.OptimizedStatus as QuestionsInstructionsAudioFileOptimizedStatus
    ,questionsInstructionsAudioFile.OptimizedBlobId as QuestionsInstructionsAudioFileOptimizedBlobId
    ,questionsInstructionsAudioFile.OptimizedFormat as QuestionsInstructionsAudioFileOptimizedFormat
    ,(select count(1) from [Education].[VocabQuestion-Active] vocabQuestion where vocabQuestion.VocabPartId = vocabPart.Id) as VocabQuestionCount
from [Education].[VocabPart-Active] vocabPart
    left join [Framework].[AudioFile-Active] previewInstructionsAudioFile on vocabPart.PreviewInstructionsAudioFileId = previewInstructionsAudioFile.Id
    left join [Framework].[AudioFile-Active] questionsInstructionsAudioFile on vocabPart.QuestionsInstructionsAudioFileId = questionsInstructionsAudioFile.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = vocabPart.AssessmentId
where assessmentAssignment.Id = @AssessmentAssignmentId
order by vocabPart.Ordinal

select
    listenQuestion.Id
    ,listenQuestion.ListenPartId
    ,listenQuestion.Text
    ,listenQuestion.IsPreview
    ,listenQuestion.PageBreakBefore
    ,listenQuestion.HasCorrectChoice
    ,listenQuestion.RequireTextInput
    ,listenQuestion.Ordinal
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
from [Education].[ListenQuestion-Active] listenQuestion
    inner join [Education].[ListenPart-Active] listenPart on listenQuestion.ListenPartId = listenPart.Id
    inner join [Education].[Assessment-Active] assessment on listenPart.AssessmentId = assessment.Id
    left join [Framework].[AudioFile-Active] audioFile on listenQuestion.AudioFileId = audioFile.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = listenPart.AssessmentId
    left join [Framework].[ImageFile-Active] imageFile on listenQuestion.ImageFileId = imageFile.Id
where assessmentAssignment.Id = @AssessmentAssignmentId
order by listenQuestion.Ordinal

select
    listenChoice.Id
    ,listenChoice.ListenQuestionId
    ,listenChoice.Text
    ,listenChoice.IsCorrect
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,listenChoice.Ordinal
from [Education].[ListenChoice-Active] listenChoice
    inner join [Education].[ListenQuestion] listenQuestion on listenChoice.ListenQuestionId = listenQuestion.Id
    inner join [Education].[ListenPart] listenPart on listenQuestion.ListenPartId = listenPart.Id
    inner join [Education].[Assessment] assessment on listenPart.AssessmentId = assessment.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = assessment.Id
    left join [Framework].[ImageFile-Active] imageFile on listenChoice.ImageFileId = imageFile.Id
    left join [Framework].[AudioFile-Active] audioFile on listenChoice.AudioFileId = audioFile.Id
where assessmentAssignment.Id = @AssessmentAssignmentId
order by listenChoice.Ordinal

select
    listenChoiceSelection.Id
    ,listenChoiceSelection.AssignmentId
    ,listenChoiceSelection.ChoiceId
    ,listenChoiceSelection.Made
    ,choice.ListenQuestionId as ChoiceListenQuestionId
    ,choice.Text as ChoiceText
from [Education].[ListenChoiceSelection-Active] listenChoiceSelection
    inner join [Education].[ListenChoice-Active] choice on listenChoiceSelection.ChoiceId = choice.Id
where listenChoiceSelection.AssignmentId = @AssessmentAssignmentId
order by listenChoiceSelection.Made desc

select
    readParagraph.Id
    ,readParagraph.ReadPartId
    ,readParagraph.Text
    ,readParagraph.Ordinal
from [Education].[ReadParagraph-Active] readParagraph
    inner join [Education].[ReadPart] readPart on readParagraph.ReadPartId = readPart.Id
    inner join [Education].[Assessment] assessment on readPart.AssessmentId = assessment.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = assessment.Id
where assessmentAssignment.Id = @AssessmentAssignmentId
order by readParagraph.Ordinal

select
    readQuestion.Id
    ,readQuestion.ReadPartId
    ,readQuestion.Text
    ,readQuestion.IsPreview
    ,readQuestion.PageBreakBefore
    ,readQuestion.HasCorrectChoice
    ,readQuestion.RequireTextInput
    ,readQuestion.Ordinal
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
from [Education].[ReadQuestion-Active] readQuestion
    inner join [Education].[ReadPart-Active] readPart on readQuestion.ReadPartId = readPart.Id
    inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
    left join [Framework].[AudioFile-Active] audioFile on readQuestion.AudioFileId = audioFile.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = assessment.Id
    left join [Framework].[ImageFile-Active] imageFile on readQuestion.ImageFileId = imageFile.Id
where assessmentAssignment.Id = @AssessmentAssignmentId
order by readQuestion.Ordinal

select
    readChoice.Id
    ,readChoice.ReadQuestionId
    ,readChoice.Text
    ,readChoice.IsCorrect
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,readChoice.Ordinal
from [Education].[ReadChoice-Active] readChoice
    inner join [Education].[ReadQuestion] readQuestion on readChoice.ReadQuestionId = readQuestion.Id
    inner join [Education].[ReadPart] readPart on readQuestion.ReadPartId = readPart.Id
    inner join [Education].[Assessment] assessment on readPart.AssessmentId = assessment.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = assessment.Id
    left join [Framework].[ImageFile-Active] imageFile on readChoice.ImageFileId = imageFile.Id
    left join [Framework].[AudioFile-Active] audioFile on readChoice.AudioFileId = audioFile.Id
where assessmentAssignment.Id = @AssessmentAssignmentId
order by readChoice.Ordinal

select
    readChoiceSelection.Id
    ,readChoiceSelection.AssignmentId
    ,readChoiceSelection.ChoiceId
    ,readChoiceSelection.Made
    ,choice.ReadQuestionId as ChoiceReadQuestionId
    ,choice.Text as ChoiceText
from [Education].[ReadChoiceSelection-Active] readChoiceSelection
    inner join [Education].[ReadChoice-Active] choice on readChoiceSelection.ChoiceId = choice.Id
where readChoiceSelection.AssignmentId = @AssessmentAssignmentId
order by readChoiceSelection.Made desc

select
    vocabQuestion.Id
    ,vocabQuestion.VocabPartId
    ,vocabQuestion.Word
    ,vocabQuestion.IsPreview
    ,vocabQuestion.PageBreakBefore
    ,vocabQuestion.Ordinal
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
from [Education].[VocabQuestion-Active] vocabQuestion
    inner join [Education].[VocabPart-Active] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id
    inner join [Education].[Assessment-Active] assessment on vocabPart.AssessmentId = assessment.Id
    left join [Framework].[AudioFile-Active] audioFile on vocabQuestion.AudioFileId = audioFile.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = assessment.Id
where assessmentAssignment.Id = @AssessmentAssignmentId
order by vocabQuestion.Ordinal

select
    vocabChoice.Id
    ,vocabChoice.VocabQuestionId
    ,vocabChoice.Word
    ,vocabChoice.IsCorrect
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,vocabChoice.Ordinal
from [Education].[VocabChoice-Active] vocabChoice
    inner join [Education].[VocabQuestion] vocabQuestion on vocabChoice.VocabQuestionId = vocabQuestion.Id
    inner join [Education].[VocabPart] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id
    inner join [Education].[Assessment] assessment on vocabPart.AssessmentId = assessment.Id
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.AssessmentId = assessment.Id
    left join [Framework].[AudioFile-Active] audioFile on vocabChoice.AudioFileId = audioFile.Id
where assessmentAssignment.Id = @AssessmentAssignmentId
order by vocabChoice.Ordinal

select
    vocabChoiceSelection.Id
    ,vocabChoiceSelection.AssignmentId
    ,vocabChoiceSelection.ChoiceId
    ,vocabChoiceSelection.Made
    ,choice.VocabQuestionId as ChoiceVocabQuestionId
    ,choice.Word as ChoiceText
from [Education].[VocabChoiceSelection-Active] vocabChoiceSelection
    inner join [Education].[VocabChoice-Active] choice on vocabChoiceSelection.ChoiceId = choice.Id
where vocabChoiceSelection.AssignmentId = @AssessmentAssignmentId
order by vocabChoiceSelection.Made desc