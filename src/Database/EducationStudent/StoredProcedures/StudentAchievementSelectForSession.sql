create proc [EducationStudent].[StudentAchievementSelectForSession] (
     @SessionId uniqueidentifier
) as

insert [Education].[AchievementViewed] (
     Id
    ,ViewedBy
)
values (
     newid()
    ,@SessionId
)

declare @studentId uniqueidentifier = (
    select top 1 student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
            and session.Id = @SessionId
)

select
     studentAchievement.Id as Id
    ,studentAchievement.StudentId as StudentId
    ,studentAchievement.Earned as Earned
    ,achievement.Id as AchievementId
    ,achievement.Title as AchievementTitle
    ,achievement.RarityId as AchievementRarityId
    ,achievement.TrophyImageId as AchievementTrophyImageId
    ,achievement.VisibleToStudents as AchievementVisibleToStudents
    ,achievementRarity.Name as AchievementRarityName
    ,achievementTrophyImage.Id as AchievementTrophyImageId
    ,achievementTrophyImage.BlobId as AchievementTrophyImageBlobId
    ,achievementTrophyImage.Name as AchievementTrophyImageName
    ,achievementTrophyImage.Format as AchievementTrophyImageFormat
    ,achievementTrophyImage.Width as AchievementTrophyImageWidth
    ,achievementTrophyImage.Height as AchievementTrophyImageHeight
    ,achievementTrophyImage.Caption as AchievementTrophyImageCaption
from [Education].[StudentAchievement-Active] studentAchievement
    inner join [Education].[Achievement-Active] achievement on studentAchievement.AchievementId = achievement.Id
    inner join [Education].[Rarity-Active] achievementRarity on achievement.RarityId = achievementRarity.Id
    left join [Framework].[ImageFile-Active] achievementTrophyImage on achievement.TrophyImageId = achievementTrophyImage.Id
where studentAchievement.StudentId = @studentId
    and achievement.VisibleToStudents = 1
order by studentAchievement.Earned desc