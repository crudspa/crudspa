create proc [SamplesCatalog].[CatalogContactSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with CatalogContactCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'First Name' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'First Name' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortField = 'Last Name' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'Last Name' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortField = 'Username' and @SortAscending = 1)
                    then userTable.Username
                end asc,
                case when (@SortField = 'Username' and @SortAscending = 0)
                    then userTable.Username
                end desc,
                case when (@SortField = 'First Name' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'First Name' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortField = 'First Name' and @SortAscending = 1)
                    then userTable.Username
                end asc,
                case when (@SortField = 'First Name' and @SortAscending = 0)
                    then userTable.Username
                end desc,
                case when (@SortField = 'Last Name' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'Last Name' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortField = 'Last Name' and @SortAscending = 1)
                    then userTable.Username
                end asc,
                case when (@SortField = 'Last Name' and @SortAscending = 0)
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
                    then catalogContact.Id
                end asc,
                case when (@SortAscending = 0)
                    then catalogContact.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,catalogContact.Id
    from [Samples].[CatalogContact-Active] catalogContact
        inner join [Framework].[Contact-Active] contact on catalogContact.ContactId = contact.Id
        left join [Framework].[User-Active] userTable on catalogContact.UserId = userTable.Id
    where 1 = 1

        and (@SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or userTable.Username like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,catalogContact.Id
    ,catalogContact.ContactId
    ,catalogContact.UserId
from CatalogContactCte cte
    inner join [Samples].[CatalogContact-Active] catalogContact on cte.Id = catalogContact.Id
    inner join [Framework].[Contact-Active] contact on catalogContact.ContactId = contact.Id
    left join [Framework].[User-Active] userTable on catalogContact.UserId = userTable.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)