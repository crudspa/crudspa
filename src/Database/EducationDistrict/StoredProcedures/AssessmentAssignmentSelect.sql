create proc [EducationDistrict].[AssessmentAssignmentSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select
    assessmentAssignment.Id as Id
    ,assessmentAssignment.AssessmentId as AssessmentId
    ,assessmentAssignment.StudentId as StudentId
    ,assessmentAssignment.Assigned as Assigned
    ,assessmentAssignment.StartAfter as StartAfter
    ,assessmentAssignment.EndBefore as EndBefore
    ,assessmentAssignment.Started as Started
    ,assessmentAssignment.Completed as Completed
    ,assessmentAssignment.Terminated as Terminated
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
from [Education].[AssessmentAssignment-Active] assessmentAssignment
    inner join [Education].[Assessment-Active] assessment on assessmentAssignment.AssessmentId = assessment.Id
    inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
where assessmentAssignment.Id = @Id
    and school.DistrictId = @districtId