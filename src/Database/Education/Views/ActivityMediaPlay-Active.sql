create view [Education].[ActivityMediaPlay-Active] as

select activityMediaPlay.Id as Id
    ,activityMediaPlay.MediaPlayId as MediaPlayId
    ,activityMediaPlay.AssignmentBatchId as AssignmentBatchId
    ,activityMediaPlay.ActivityId as ActivityId
    ,activityMediaPlay.ActivityChoiceId as ActivityChoiceId
from [Education].[ActivityMediaPlay] activityMediaPlay
where 1=1
    and activityMediaPlay.IsDeleted = 0
    and activityMediaPlay.VersionOf = activityMediaPlay.Id