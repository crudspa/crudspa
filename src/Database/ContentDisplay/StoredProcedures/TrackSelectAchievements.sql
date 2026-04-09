create proc [ContentDisplay].[TrackSelectAchievements] (
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
from [Content].[Track-Active] track
    inner join [Content].[Course-Active] course on course.TrackId = track.Id
    left join [Content].[Achievement-Active] generatesAchievement on track.GeneratesAchievementId = generatesAchievement.Id
    left join [Content].[Achievement-Active] requiresAchievement on track.RequiresAchievementId = requiresAchievement.Id
where course.Id = @CourseId