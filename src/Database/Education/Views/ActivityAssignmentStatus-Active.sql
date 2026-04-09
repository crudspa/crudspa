create view [Education].[ActivityAssignmentStatus-Active] as

select activityAssignmentStatus.Id as Id
    ,activityAssignmentStatus.Name as Name
    ,activityAssignmentStatus.Ordinal as Ordinal
from [Education].[ActivityAssignmentStatus] activityAssignmentStatus
where 1=1
    and activityAssignmentStatus.IsDeleted = 0