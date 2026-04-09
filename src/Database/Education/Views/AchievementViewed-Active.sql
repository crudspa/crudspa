create view [Education].[AchievementViewed-Active] as

select achievementViewed.Id as Id
    ,achievementViewed.Viewed as Viewed
    ,achievementViewed.ViewedBy as ViewedBy
    ,achievementViewed.StudentAchievementId as StudentAchievementId
from [Education].[AchievementViewed] achievementViewed
where 1=1