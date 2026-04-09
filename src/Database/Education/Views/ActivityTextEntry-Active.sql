create view [Education].[ActivityTextEntry-Active] as

select activityTextEntry.Id as Id
    ,activityTextEntry.AssignmentId as AssignmentId
    ,activityTextEntry.Text as Text
    ,activityTextEntry.Made as Made
    ,activityTextEntry.Ordinal as Ordinal
from [Education].[ActivityTextEntry] activityTextEntry
where 1=1
    and activityTextEntry.IsDeleted = 0
    and activityTextEntry.VersionOf = activityTextEntry.Id