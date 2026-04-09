create proc [EducationPublisher].[PublisherContactSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with PublisherContactCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'First' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'First' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortField = 'Last' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'Last' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortField = 'Username' and @SortAscending = 1)
                    then userTable.Username
                end asc,
                case when (@SortField = 'Username' and @SortAscending = 0)
                    then userTable.Username
                end desc,
                case when (@SortField = 'First' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'First' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortField = 'First' and @SortAscending = 1)
                    then userTable.Username
                end asc,
                case when (@SortField = 'First' and @SortAscending = 0)
                    then userTable.Username
                end desc,
                case when (@SortField = 'Last' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'Last' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortField = 'Last' and @SortAscending = 1)
                    then userTable.Username
                end asc,
                case when (@SortField = 'Last' and @SortAscending = 0)
                    then userTable.Username
                end desc,
                case when (@SortField = 'Username' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'Username' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortField = 'Username' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'Username' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortAscending = 1)
                    then publisherContact.Id
                end asc,
                case when (@SortAscending = 0)
                    then publisherContact.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,publisherContact.Id
    from [Education].[PublisherContact-Active] publisherContact
        inner join [Framework].[Contact-Active] contact on publisherContact.ContactId = contact.Id
        inner join [Education].[Publisher-Active] publisher on publisherContact.PublisherId = publisher.Id
        left join [Framework].[User-Active] userTable on publisherContact.UserId = userTable.Id
    where 1 = 1
        and publisher.OrganizationId = @organizationId
        and publisher.Id = @publisherId
        and (@SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or userTable.Username like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,publisherContact.Id
    ,publisherContact.UserId
    ,publisherContact.ContactId
from PublisherContactCte cte
    inner join [Education].[PublisherContact-Active] publisherContact on cte.Id = publisherContact.Id
    inner join [Framework].[Contact-Active] contact on publisherContact.ContactId = contact.Id
    inner join [Education].[Publisher-Active] publisher on publisherContact.PublisherId = publisher.Id
    left join [Framework].[User-Active] userTable on publisherContact.UserId = userTable.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)