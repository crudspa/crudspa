create proc [ContentDesign].[ThreadSelectWhereForForum] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@PostedStart datetimeoffset(7)
    ,@PostedEnd datetimeoffset(7)
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

;with ThreadCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Posted' and @SortAscending = 1)
                    then comment.Posted
                end asc,
                case when (@SortField = 'Posted' and @SortAscending = 0)
                    then comment.Posted
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then thread.Title
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then thread.Title
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,thread.Id
    from [Content].[Thread-Active] thread
        inner join [Content].[Comment-Active] comment on thread.CommentId = comment.Id
        inner join [Framework].[Contact-Active] byTable on comment.ById = byTable.Id
        inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where 1 = 1
        and thread.ForumId = @ForumId
        and organization.Id = @organizationId
        and (@SearchText is null
            or thread.Title like '%' + @SearchText + '%'
        )
        and (@PostedStart is null or comment.Posted >= @PostedStart)
        and (@PostedEnd is null or comment.Posted < @PostedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,thread.Id
    ,thread.ForumId
    ,forum.Title as ForumTitle
    ,thread.Title
    ,thread.Pinned
    ,comment.Id as CommentId
    ,comment.Body as CommentBody
    ,comment.ById as CommentById
    ,byTable.FirstName as ContactFirstName
    ,comment.Posted as CommentPosted
    ,comment.Edited as CommentEdited
    ,(select count(1) from [Content].[Comment-Active] where ThreadId = thread.Id) as CommentCount
from ThreadCte cte
    inner join [Content].[Thread-Active] thread on cte.Id = thread.Id
    inner join [Content].[Comment-Active] comment on thread.CommentId = comment.Id
    inner join [Framework].[Contact-Active] byTable on comment.ById = byTable.Id
    inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)