create proc [EducationPublisher].[GameSelectWhereForBook] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Status Framework.IdList readonly
    ,@Grades Framework.IdList readonly
    ,@AssessmentLevels Framework.IdList readonly
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @statusCount int = (select count(1) from @Status)
declare @gradesCount int = (select count(1) from @Grades)
declare @assessmentLevelsCount int = (select count(1) from @AssessmentLevels)

;with GameCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Key' and @SortAscending = 1)
                    then game.[Key]
                end asc,
                case when (@SortField = 'Key' and @SortAscending = 0)
                    then game.[Key]
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then game.Title
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then game.Title
                end desc,
                case when (@SortField = 'Key' and @SortAscending = 1)
                    then game.Title
                end asc,
                case when (@SortField = 'Key' and @SortAscending = 0)
                    then game.Title
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then game.[Key]
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then game.[Key]
                end desc,
                case when (@SortAscending = 1)
                    then game.Id
                end asc,
                case when (@SortAscending = 0)
                    then game.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,game.Id
    from [Education].[Game-Active] game
        inner join [Education].[AssessmentLevel-Active] assessmentLevel on game.AssessmentLevelId = assessmentLevel.Id
        inner join [Education].[Book-Active] book on game.BookId = book.Id
        left join [Education].[Achievement-Active] generatesAchievement on game.GeneratesAchievementId = generatesAchievement.Id
        inner join [Education].[Grade-Active] grade on game.GradeId = grade.Id
        left join [Framework].[Icon-Active] icon on game.IconId = icon.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
        left join [Education].[Achievement-Active] requiresAchievement on game.RequiresAchievementId = requiresAchievement.Id
        inner join [Framework].[ContentStatus-Active] status on game.StatusId = status.Id
    where 1 = 1
        and game.BookId = @BookId
        and organization.Id = @organizationId
        and (@SearchText is null
            or game.[Key] like '%' + @SearchText + '%'
            or game.Title like '%' + @SearchText + '%'
        )
        and (@statusCount = 0 or game.StatusId in (select Id from @Status))
        and (@gradesCount = 0 or game.GradeId in (select Id from @Grades))
        and (@assessmentLevelsCount = 0 or game.AssessmentLevelId in (select Id from @AssessmentLevels))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,game.Id
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
from GameCte cte
    inner join [Education].[Game-Active] game on cte.Id = game.Id
    inner join [Education].[AssessmentLevel-Active] assessmentLevel on game.AssessmentLevelId = assessmentLevel.Id
    inner join [Education].[Book-Active] book on game.BookId = book.Id
    left join [Education].[Achievement-Active] generatesAchievement on game.GeneratesAchievementId = generatesAchievement.Id
    inner join [Education].[Grade-Active] grade on game.GradeId = grade.Id
    left join [Framework].[Icon-Active] icon on game.IconId = icon.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    left join [Education].[Achievement-Active] requiresAchievement on game.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on game.StatusId = status.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)