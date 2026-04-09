create proc [EducationProvider].[PublisherSelectWhere] (
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

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with PublisherCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then organization.Name
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then organization.Name
                end desc,
                case when (@SortAscending = 1)
                    then publisher.Id
                end asc,
                case when (@SortAscending = 0)
                    then publisher.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Framework].[Organization-Active] organization on publisher.OrganizationId = organization.Id
        inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
    where 1 = 1
        and provider.Id = @providerId
        and (@SearchText is null
            or organization.Name like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,publisher.Id
    ,publisher.OrganizationId
    ,(select count(1) from [Education].[PublisherContact-Active] where PublisherId = publisher.Id) as PublisherContactCount
from PublisherCte cte
    inner join [Education].[Publisher-Active] publisher on cte.Id = publisher.Id
    inner join [Framework].[Organization-Active] organization on publisher.OrganizationId = organization.Id
    inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)