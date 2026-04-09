create proc [EducationProvider].[ProviderContactSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
) as

declare @providerId uniqueidentifier = (
    select top 1 provider.Id
    from [Education].[Provider-Active] provider
        inner join [Education].[ProviderContact-Active] providerContact on providerContact.ProviderId = provider.Id
        inner join [Framework].[User-Active] userTable on providerContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with ProviderContactCte
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
                    then providerContact.Id
                end asc,
                case when (@SortAscending = 0)
                    then providerContact.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,providerContact.Id
    from [Education].[ProviderContact-Active] providerContact
        inner join [Framework].[Contact-Active] contact on providerContact.ContactId = contact.Id
        inner join [Education].[Provider-Active] provider on providerContact.ProviderId = provider.Id
        left join [Framework].[User-Active] userTable on providerContact.UserId = userTable.Id
    where 1 = 1
        and provider.Id = @providerId
        and provider.OrganizationId = @organizationId
        and (@SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or userTable.Username like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,providerContact.Id
    ,providerContact.UserId
    ,providerContact.ContactId
from ProviderContactCte cte
    inner join [Education].[ProviderContact-Active] providerContact on cte.Id = providerContact.Id
    inner join [Framework].[Contact-Active] contact on providerContact.ContactId = contact.Id
    inner join [Education].[Provider-Active] provider on providerContact.ProviderId = provider.Id
    left join [Framework].[User-Active] userTable on providerContact.UserId = userTable.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)