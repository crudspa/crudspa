create view [Education].[ReadChoiceSelection-Active] as

select readChoiceSelection.Id as Id
    ,readChoiceSelection.AssignmentId as AssignmentId
    ,readChoiceSelection.ChoiceId as ChoiceId
    ,readChoiceSelection.Made as Made
from [Education].[ReadChoiceSelection] readChoiceSelection
where 1=1
    and readChoiceSelection.IsDeleted = 0