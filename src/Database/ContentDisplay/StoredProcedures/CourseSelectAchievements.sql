create proc [ContentDisplay].[CourseSelectAchievements] (
     @CourseId uniqueidentifier
) as

select
    course.Id
    ,course.Title
    ,course.Description
    ,generatesAchievement.Id as GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,generatesAchievement.Description as GeneratesAchievementDescription
    ,generatesAchievement.ImageId as GeneratesAchievementImageId
    ,requiresAchievement.Id as RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,requiresAchievement.Description as RequiresAchievementDescription
    ,requiresAchievement.ImageId as RequiresAchievementImageId
from [Content].[Course-Active] course
    left join [Content].[Achievement-Active] generatesAchievement on course.GeneratesAchievementId = generatesAchievement.Id
    left join [Content].[Achievement-Active] requiresAchievement on course.RequiresAchievementId = requiresAchievement.Id
where course.Id = @CourseId