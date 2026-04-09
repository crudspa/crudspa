create proc [EducationPublisher].[DistrictSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
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

;with DistrictCte
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
                    then district.Id
                end asc,
                case when (@SortAscending = 0)
                    then district.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,district.Id
    from [Education].[District-Active] district
        left join [Framework].[UsaPostal-Active] address on district.AddressId = address.Id
        inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
        inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    where 1 = 1
        and publisher.Id = @publisherId
        and (@SearchText is null
            or organization.Name like '%' + @SearchText + '%'
            or district.StudentIdNumberLabel like '%' + @SearchText + '%'
            or district.AssessmentExplainer like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,district.Id
    ,district.StudentIdNumberLabel
    ,district.AssessmentExplainer
    ,district.OrganizationId
    ,district.AddressId
    ,(select count(1) from [Education].[DistrictContact-Active] where DistrictId = district.Id) as DistrictContactCount
    ,(select count(1) from [Education].[Community-Active] where DistrictId = district.Id) as CommunityCount
    ,(select count(1) from [Education].[School-Active] where DistrictId = district.Id) as SchoolCount
from DistrictCte cte
    inner join [Education].[District-Active] district on cte.Id = district.Id
    left join [Framework].[UsaPostal-Active] address on district.AddressId = address.Id
    inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)