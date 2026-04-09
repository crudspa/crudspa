create proc [EducationPublisher].[ActivitySelect] (
     @Id uniqueidentifier
) as

select
    activity.*
from [Education].[Activity-Deep] activity
where activity.Id = @Id

select
    activityChoice.*
from [Education].[ActivityChoice-Deep] activityChoice
where activityChoice.ActivityId = @Id