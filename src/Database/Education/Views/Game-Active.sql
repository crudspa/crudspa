create view [Education].[Game-Active] as

select game.Id as Id
    ,game.BookId as BookId
    ,game.[Key] as [Key]
    ,game.Title as Title
    ,game.IconName as IconName
    ,game.IconId as IconId
    ,game.StatusId as StatusId
    ,game.GradeId as GradeId
    ,game.AssessmentLevelId as AssessmentLevelId
    ,game.RequiresAchievementId as RequiresAchievementId
    ,game.GeneratesAchievementId as GeneratesAchievementId
from [Education].[Game] game
where 1=1
    and game.IsDeleted = 0
    and game.VersionOf = game.Id