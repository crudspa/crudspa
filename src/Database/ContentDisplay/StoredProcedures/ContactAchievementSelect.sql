create proc [ContentDisplay].[ContactAchievementSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

begin transaction
    insert [Content].[AchievementViewed] (Id, ViewedBy, ContactAchievementId)
    values (newid(), @SessionId, @Id)
commit transaction

select
    contactAchievement.Id
    ,contactAchievement.ContactId
    ,contactAchievement.Earned
    ,achievement.Id as AchievementId
    ,achievement.Title as AchievementTitle
    ,achievement.Description as AchievementDescription
    ,achievement.ImageId as AchievementImageId
    ,achievementImage.Id as AchievementImageId
    ,achievementImage.BlobId as AchievementImageBlobId
    ,achievementImage.Name as AchievementImageName
    ,achievementImage.Format as AchievementImageFormat
    ,achievementImage.Width as AchievementImageWidth
    ,achievementImage.Height as AchievementImageHeight
    ,achievementImage.Caption as AchievementImageCaption
from [Content].[ContactAchievement-Active] contactAchievement
    inner join [Content].[Achievement-Active] achievement on contactAchievement.AchievementId = achievement.Id
    inner join [Framework].[ImageFile-Active] achievementImage on achievement.ImageId = achievementImage.Id
where contactAchievement.Id = @Id