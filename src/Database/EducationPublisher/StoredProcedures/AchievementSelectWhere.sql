create proc [EducationPublisher].[AchievementSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Rarities Framework.IdList readonly
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
declare @raritiesCount int = (select count(1) from @Rarities)

;with AchievementCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then achievement.Title
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then achievement.Title
                end desc,
                case when (@SortField = 'Rarity' and @SortAscending = 1)
                    then rarity.Name
                end asc,
                case when (@SortField = 'Rarity' and @SortAscending = 0)
                    then rarity.Name
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then rarity.Name
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then rarity.Name
                end desc,
                case when (@SortField = 'Rarity' and @SortAscending = 1)
                    then achievement.Title
                end asc,
                case when (@SortField = 'Rarity' and @SortAscending = 0)
                    then achievement.Title
                end desc,
                case when (@SortAscending = 1)
                    then achievement.Id
                end asc,
                case when (@SortAscending = 0)
                    then achievement.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,achievement.Id
    from [Education].[Achievement-Active] achievement
        inner join [Framework].[Organization-Active] organization on achievement.OwnerId = organization.Id
        inner join [Education].[Rarity-Active] rarity on achievement.RarityId = rarity.Id
        left join [Framework].[ImageFile-Active] trophyImage on achievement.TrophyImageId = trophyImage.Id
    where 1 = 1
        and organization.Id = @organizationId
        and (@SearchText is null
            or achievement.Title like '%' + @SearchText + '%'
        )
        and (@raritiesCount = 0 or achievement.RarityId in (select Id from @Rarities))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,achievement.Id
    ,achievement.Title
    ,achievement.RarityId
    ,rarity.Name as RarityName
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
    ,achievement.VisibleToStudents
from AchievementCte cte
    inner join [Education].[Achievement-Active] achievement on cte.Id = achievement.Id
    inner join [Framework].[Organization-Active] organization on achievement.OwnerId = organization.Id
    inner join [Education].[Rarity-Active] rarity on achievement.RarityId = rarity.Id
    left join [Framework].[ImageFile-Active] trophyImage on achievement.TrophyImageId = trophyImage.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)