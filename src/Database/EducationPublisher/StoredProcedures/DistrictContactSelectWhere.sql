create proc [EducationPublisher].[DistrictContactSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Districts Framework.IdList readonly
) as

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
declare @districtsCount int = (select count(1) from @Districts)

;with DistrictContactCte
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
                    then districtContact.Id
                end asc,
                case when (@SortAscending = 0)
                    then districtContact.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,districtContact.Id
    from [Education].[DistrictContact-Active] districtContact
        inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
        inner join [Education].[District-Active] district on districtContact.DistrictId = district.Id
        inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
        left join [Framework].[User-Active] userTable on districtContact.UserId = userTable.Id
    where 1 = 1
        and publisher.Id = @publisherId
        and (@SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or userTable.Username like '%' + @SearchText + '%'
        )
        and (@districtsCount = 0 or districtContact.DistrictId in (select Id from @Districts))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,districtContact.Id
    ,districtContact.DistrictId
    ,districtContact.Title
    ,districtContact.UserId
    ,districtContact.ContactId
    ,districtOrganization.Name as DistrictName
from DistrictContactCte cte
    inner join [Education].[DistrictContact-Active] districtContact on cte.Id = districtContact.Id
    inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
    inner join [Education].[District-Active] district on districtContact.DistrictId = district.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    left join [Framework].[User-Active] userTable on districtContact.UserId = userTable.Id
    inner join [Framework].[Organization-Active] districtOrganization on district.OrganizationId = districtOrganization.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)