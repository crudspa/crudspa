create proc [EducationSchool].[SchoolContactSelectWhere] (
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

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with SchoolContactCte
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
                    then schoolContact.Id
                end asc,
                case when (@SortAscending = 0)
                    then schoolContact.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,schoolContact.Id
    from [Education].[SchoolContact-Active] schoolContact
        inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
        inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
        inner join [Education].[Title-Active] title on schoolContact.TitleId = title.Id
        left join [Framework].[User-Active] userTable on schoolContact.UserId = userTable.Id
    where 1 = 1
        and school.OrganizationId = @organizationId
        and school.Id = @schoolId
        and (@SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or userTable.Username like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,schoolContact.Id
    ,schoolContact.TitleId
    ,title.Name as TitleName
    ,schoolContact.TestAccount
    ,schoolContact.ContactId
    ,schoolContact.UserId
from SchoolContactCte cte
    inner join [Education].[SchoolContact-Active] schoolContact on cte.Id = schoolContact.Id
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    inner join [Education].[Title-Active] title on schoolContact.TitleId = title.Id
    left join [Framework].[User-Active] userTable on schoolContact.UserId = userTable.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)