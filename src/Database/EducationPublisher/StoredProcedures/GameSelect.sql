create proc [EducationPublisher].[GameSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     game.Id
    ,game.BookId
    ,book.[Key] as BookKey
    ,game.[Key]
    ,game.Title
    ,game.StatusId
    ,status.Name as StatusName
    ,game.IconId
    ,icon.Name as IconName
    ,game.GradeId
    ,grade.Name as GradeName
    ,game.AssessmentLevelId
    ,assessmentLevel.[Key] as AssessmentLevelKey
    ,game.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,game.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,(select count(1) from [Education].[GameSection-Active] where GameId = game.Id) as GameSectionCount
from [Education].[Game-Active] game
    inner join [Education].[AssessmentLevel-Active] assessmentLevel on game.AssessmentLevelId = assessmentLevel.Id
    inner join [Education].[Book-Active] book on game.BookId = book.Id
    left join [Education].[Achievement-Active] generatesAchievement on game.GeneratesAchievementId = generatesAchievement.Id
    inner join [Education].[Grade-Active] grade on game.GradeId = grade.Id
    left join [Framework].[Icon-Active] icon on game.IconId = icon.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    left join [Education].[Achievement-Active] requiresAchievement on game.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on game.StatusId = status.Id
where game.Id = @Id
    and organization.Id = @organizationId