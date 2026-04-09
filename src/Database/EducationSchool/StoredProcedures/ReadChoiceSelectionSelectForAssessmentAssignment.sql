create proc [EducationSchool].[ReadChoiceSelectionSelectForAssessmentAssignment] (
     @AssessmentAssignmentId uniqueidentifier
) as

select
    readChoiceSelection.Id as Id
    ,readChoiceSelection.AssignmentId as AssignmentId
    ,readChoiceSelection.ChoiceId as ChoiceId
    ,readChoiceSelection.Made as Made
    ,choice.ReadQuestionId as ChoiceReadQuestionId
    ,choice.Text as ChoiceText
from [Education].[ReadChoiceSelection-Active] readChoiceSelection
    inner join [Education].[ReadChoice-Active] choice on readChoiceSelection.ChoiceId = choice.Id
where readChoiceSelection.AssignmentId = @AssessmentAssignmentId
order by readChoiceSelection.Made desc