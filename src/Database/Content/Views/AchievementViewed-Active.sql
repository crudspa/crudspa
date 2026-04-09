create view [Content].[AchievementViewed-Active] as

select achievementViewed.Id as Id
    ,achievementViewed.Viewed as Viewed
    ,achievementViewed.ViewedBy as ViewedBy
    ,achievementViewed.ContactAchievementId as ContactAchievementId
from [Content].[AchievementViewed] achievementViewed
where 1=1