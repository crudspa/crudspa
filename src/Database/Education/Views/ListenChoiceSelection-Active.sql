create view [Education].[ListenChoiceSelection-Active] as

select listenChoiceSelection.Id as Id
    ,listenChoiceSelection.AssignmentId as AssignmentId
    ,listenChoiceSelection.ChoiceId as ChoiceId
    ,listenChoiceSelection.Made as Made
from [Education].[ListenChoiceSelection] listenChoiceSelection
where 1=1
    and listenChoiceSelection.IsDeleted = 0