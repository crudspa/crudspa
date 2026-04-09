create proc [ContentDesign].[PostSelectWhereForBlog] (
     @SessionId uniqueidentifier
    ,@BlogId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Status Framework.IdList readonly
    ,@PublishedStart datetimeoffset(7)
    ,@PublishedEnd datetimeoffset(7)
    ,@RevisedStart datetimeoffset(7)
    ,@RevisedEnd datetimeoffset(7)
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

;with PostCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Published' and @SortAscending = 1)
                    then post.Published
                end asc,
                case when (@SortField = 'Published' and @SortAscending = 0)
                    then post.Published
                end desc,
                case when (@SortField = 'Revised' and @SortAscending = 1)
                    then post.Revised
                end asc,
                case when (@SortField = 'Revised' and @SortAscending = 0)
                    then post.Revised
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then post.Title
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then post.Title
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then post.Author
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then post.Author
                end desc,
                case when (@SortField = 'Published' and @SortAscending = 1)
                    then post.Revised
                end asc,
                case when (@SortField = 'Published' and @SortAscending = 0)
                    then post.Revised
                end desc,
                case when (@SortField = 'Published' and @SortAscending = 1)
                    then post.Title
                end asc,
                case when (@SortField = 'Published' and @SortAscending = 0)
                    then post.Title
                end desc,
                case when (@SortField = 'Published' and @SortAscending = 1)
                    then post.Author
                end asc,
                case when (@SortField = 'Published' and @SortAscending = 0)
                    then post.Author
                end desc,
                case when (@SortField = 'Revised' and @SortAscending = 1)
                    then post.Published
                end asc,
                case when (@SortField = 'Revised' and @SortAscending = 0)
                    then post.Published
                end desc,
                case when (@SortField = 'Revised' and @SortAscending = 1)
                    then post.Title
                end asc,
                case when (@SortField = 'Revised' and @SortAscending = 0)
                    then post.Title
                end desc,
                case when (@SortField = 'Revised' and @SortAscending = 1)
                    then post.Author
                end asc,
                case when (@SortField = 'Revised' and @SortAscending = 0)
                    then post.Author
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then post.Published
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then post.Published
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then post.Revised
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then post.Revised
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then post.Author
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then post.Author
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then post.Published
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then post.Published
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then post.Revised
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then post.Revised
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then post.Title
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then post.Title
                end desc,
                case when (@SortAscending = 1)
                    then post.Id
                end asc,
                case when (@SortAscending = 0)
                    then post.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,post.Id
    from [Content].[Post-Active] post
        inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
        inner join [Content].[Page-Active] page on post.PageId = page.Id
        inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
        inner join [Framework].[ContentStatus-Active] status on post.StatusId = status.Id
        inner join [Content].[PageType-Active] type on page.TypeId = type.Id
    where 1 = 1
        and post.BlogId = @BlogId
        and organization.Id = @organizationId
        and (@SearchText is null
            or post.Title like '%' + @SearchText + '%'
            or post.Author like '%' + @SearchText + '%'
        )
        and (@statusCount = 0 or post.StatusId in (select Id from @Status))
        and (@PublishedStart is null or post.Published >= @PublishedStart)
        and (@PublishedEnd is null or post.Published < @PublishedEnd)
        and (@RevisedStart is null or post.Revised >= @RevisedStart)
        and (@RevisedEnd is null or post.Revised < @RevisedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,post.Id
    ,post.BlogId
    ,post.Title
    ,post.StatusId
    ,status.Name as StatusName
    ,post.Author
    ,post.Published
    ,post.Revised
    ,post.CommentRule
    ,page.Id as PageId
    ,page.TypeId as PageTypeId
    ,type.Name as PageTypeName
    ,(select count(1) from [Content].[Comment-Active] where PostId = post.Id) as CommentCount
    ,(select count(1) from [Content].[PostReaction-Active] where PostId = post.Id) as PostReactionCount
    ,(select count(1) from [Content].[PostTag-Active] where PostId = post.Id) as PostTagCount
    ,(select count(1) from [Content].[Section-Active] where PageId = post.PageId) as SectionCount
from PostCte cte
    inner join [Content].[Post-Active] post on cte.Id = post.Id
    inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
    inner join [Content].[Page-Active] page on post.PageId = page.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on post.StatusId = status.Id
    inner join [Content].[PageType-Active] type on page.TypeId = type.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)