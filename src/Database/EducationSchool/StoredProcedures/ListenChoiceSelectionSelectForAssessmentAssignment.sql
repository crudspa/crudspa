create proc [EducationSchool].[ListenChoiceSelectionSelectForAssessmentAssignment] (
     @AssessmentAssignmentId uniqueidentifier
) as

select
    listenChoiceSelection.Id as Id
    ,listenChoiceSelection.AssignmentId as AssignmentId
    ,listenChoiceSelection.ChoiceId as ChoiceId
    ,listenChoiceSelection.Made as Made
    ,choice.ListenQuestionId as ChoiceListenQuestionId
    ,choice.Text as ChoiceText
from [Education].[ListenChoiceSelection-Active] listenChoiceSelection
    inner join [Education].[ListenChoice-Active] choice on listenChoiceSelection.ChoiceId = choice.Id
where listenChoiceSelection.AssignmentId = @AssessmentAssignmentId
order by listenChoiceSelection.Made desc