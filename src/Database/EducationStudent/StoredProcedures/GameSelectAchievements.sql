create proc [EducationStudent].[GameSelectAchievements] (
     @GameId uniqueidentifier
) as

select
    game.Id as Id
    ,game.Title as Title
    ,generatesAchievement.Id as GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,generatesAchievement.RarityId as GeneratesAchievementRarityId
    ,generatesAchievement.TrophyImageId as GeneratesAchievementTrophyImageId
    ,generatesAchievement.VisibleToStudents as GeneratesAchievementVisibleToStudents
    ,requiresAchievement.Id as RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,requiresAchievement.RarityId as RequiresAchievementRarityId
    ,requiresAchievement.TrophyImageId as RequiresAchievementTrophyImageId
    ,requiresAchievement.VisibleToStudents as RequiresAchievementVisibleToStudents
from [Education].[Game-Active] game
    left join [Education].[Achievement-Active] generatesAchievement on game.GeneratesAchievementId = generatesAchievement.Id
    left join [Education].[Achievement-Active] requiresAchievement on game.RequiresAchievementId = requiresAchievement.Id
where game.Id = @GameId