create proc [EducationStudent].[GameSelect] (
     @Id uniqueidentifier
) as

select
    game.Id as Id
    ,game.BookId as BookId
    ,game.[Key] as [Key]
    ,game.Title as Title
    ,icon.CssClass as IconName
    ,game.StatusId as StatusId
    ,game.GradeId as GradeId
    ,game.AssessmentLevelId as AssessmentLevelId
    ,game.RequiresAchievementId as RequiresAchievementId
    ,game.GeneratesAchievementId as GeneratesAchievementId
from [Education].[Game-Active] game
    left join [Framework].[Icon-Active] icon on game.IconId = icon.Id
where game.Id = @Id