create view [Education].[ObjectiveViewed-Active] as

select objectiveViewed.Id as Id
    ,objectiveViewed.ObjectiveId as ObjectiveId
from [Education].[ObjectiveViewed] objectiveViewed
where 1=1
    and objectiveViewed.IsDeleted = 0