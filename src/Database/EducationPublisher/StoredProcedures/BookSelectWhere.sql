create proc [EducationPublisher].[BookSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Seasons Framework.IdList readonly
    ,@Status Framework.IdList readonly
    ,@Categories Framework.IdList readonly
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
declare @seasonsCount int = (select count(1) from @Seasons)
declare @statusCount int = (select count(1) from @Status)
declare @categoriesCount int = (select count(1) from @Categories)

;with BookCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then book.Title
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then book.Title
                end desc,
                case when (@SortField = 'Key' and @SortAscending = 1)
                    then book.[Key]
                end asc,
                case when (@SortField = 'Key' and @SortAscending = 0)
                    then book.[Key]
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then book.Author
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then book.Author
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then book.[Key]
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then book.[Key]
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then book.Author
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then book.Author
                end desc,
                case when (@SortField = 'Key' and @SortAscending = 1)
                    then book.Title
                end asc,
                case when (@SortField = 'Key' and @SortAscending = 0)
                    then book.Title
                end desc,
                case when (@SortField = 'Key' and @SortAscending = 1)
                    then book.Author
                end asc,
                case when (@SortField = 'Key' and @SortAscending = 0)
                    then book.Author
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then book.Title
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then book.Title
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then book.[Key]
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then book.[Key]
                end desc,
                case when (@SortAscending = 1)
                    then book.Id
                end asc,
                case when (@SortAscending = 0)
                    then book.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,book.Id
    from [Education].[Book-Active] book
        left join [Education].[BookCategory-Active] category on book.CategoryId = category.Id
        inner join [Framework].[ImageFile-Active] coverImage on book.CoverImageId = coverImage.Id
        left join [Framework].[ImageFile-Active] guideImage on book.GuideImageId = guideImage.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
        left join [Education].[Achievement-Active] requiresAchievement on book.RequiresAchievementId = requiresAchievement.Id
        inner join [Education].[BookSeason-Active] season on book.SeasonId = season.Id
        inner join [Framework].[ContentStatus-Active] status on book.StatusId = status.Id
    where 1 = 1
        and organization.Id = @organizationId
        and (@SearchText is null
            or book.[Key] like '%' + @SearchText + '%'
            or book.Isbn like '%' + @SearchText + '%'
            or book.Title like '%' + @SearchText + '%'
            or book.Author like '%' + @SearchText + '%'
            or book.Lexile like '%' + @SearchText + '%'
        )
        and (@seasonsCount = 0 or book.SeasonId in (select Id from @Seasons))
        and (@statusCount = 0 or book.StatusId in (select Id from @Status))
        and (@categoriesCount = 0 or book.CategoryId in (select Id from @Categories))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,book.Id
    ,book.Title
    ,book.StatusId
    ,status.Name as StatusName
    ,book.[Key]
    ,book.Author
    ,book.Isbn
    ,book.Lexile
    ,book.SeasonId
    ,season.Name as SeasonName
    ,book.CategoryId
    ,category.Name as CategoryName
    ,book.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,book.Summary
    ,coverImage.Id as CoverImageId
    ,coverImage.BlobId as CoverImageBlobId
    ,coverImage.Name as CoverImageName
    ,coverImage.Format as CoverImageFormat
    ,coverImage.Width as CoverImageWidth
    ,coverImage.Height as CoverImageHeight
    ,coverImage.Caption as CoverImageCaption
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
    ,(select count(1) from [Education].[Chapter-Active] where BookId = book.Id) as ChapterCount
    ,(select count(1) from [Education].[Game-Active] where BookId = book.Id) as GameCount
    ,(select count(1) from [Education].[Module-Active] where BookId = book.Id) as ModuleCount
    ,(select count(1) from [Education].[Trifold-Active] where BookId = book.Id) as TrifoldCount
    ,(select count(1) from [Content].[Page-Active] where BinderId = book.PrefaceBinderId) as PrefaceCount
from BookCte cte
    inner join [Education].[Book-Active] book on cte.Id = book.Id
    left join [Education].[BookCategory-Active] category on book.CategoryId = category.Id
    inner join [Framework].[ImageFile-Active] coverImage on book.CoverImageId = coverImage.Id
    left join [Framework].[ImageFile-Active] guideImage on book.GuideImageId = guideImage.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    left join [Education].[Achievement-Active] requiresAchievement on book.RequiresAchievementId = requiresAchievement.Id
    inner join [Education].[BookSeason-Active] season on book.SeasonId = season.Id
    inner join [Framework].[ContentStatus-Active] status on book.StatusId = status.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)