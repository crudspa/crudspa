create proc [EducationPublisher].[AchievementSelectNames] as

set nocount on
select
    achievement.Id
    ,achievement.Title
from [Education].[Achievement-Active] achievement
order by achievement.Title