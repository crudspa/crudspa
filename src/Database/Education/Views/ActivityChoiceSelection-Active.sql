create view [Education].[ActivityChoiceSelection-Active] as

select activityChoiceSelection.Id as Id
    ,activityChoiceSelection.AssignmentId as AssignmentId
    ,activityChoiceSelection.ChoiceId as ChoiceId
    ,activityChoiceSelection.Made as Made
    ,activityChoiceSelection.TargetChoiceId as TargetChoiceId
from [Education].[ActivityChoiceSelection] activityChoiceSelection
where 1=1
    and activityChoiceSelection.IsDeleted = 0
    and activityChoiceSelection.VersionOf = activityChoiceSelection.Id