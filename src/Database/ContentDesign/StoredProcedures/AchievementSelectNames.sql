create proc [ContentDesign].[AchievementSelectNames] (
     @PortalId uniqueidentifier
) as

set nocount on

select
     achievement.Id
    ,achievement.Title
from [Content].[Achievement-Active] achievement
    where achievement.PortalId = @PortalId
order by achievement.Title