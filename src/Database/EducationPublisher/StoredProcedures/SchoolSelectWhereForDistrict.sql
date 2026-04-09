create proc [EducationPublisher].[SchoolSelectWhereForDistrict] (
     @SessionId uniqueidentifier
    ,@DistrictId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Communities Framework.IdList readonly
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
declare @communitiesCount int = (select count(1) from @Communities)

;with SchoolCte
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
                case when (@SortField = 'Key' and @SortAscending = 1)
                    then school.[Key]
                end asc,
                case when (@SortField = 'Key' and @SortAscending = 0)
                    then school.[Key]
                end desc,
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then school.[Key]
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then school.[Key]
                end desc,
                case when (@SortField = 'Key' and @SortAscending = 1)
                    then organization.Name
                end asc,
                case when (@SortField = 'Key' and @SortAscending = 0)
                    then organization.Name
                end desc,
                case when (@SortAscending = 1)
                    then school.Id
                end asc,
                case when (@SortAscending = 0)
                    then school.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,school.Id
    from [Education].[School-Active] school
        left join [Framework].[UsaPostal-Active] address on school.AddressId = address.Id
        left join [Education].[Community-Active] community on school.CommunityId = community.Id
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
        inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
        inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    where 1 = 1
        and school.DistrictId = @DistrictId
        and publisher.Id = @publisherId
        and (@SearchText is null
            or organization.Name like '%' + @SearchText + '%'
            or school.[Key] like '%' + @SearchText + '%'
        )
        and (@communitiesCount = 0 or school.CommunityId in (select Id from @Communities))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,school.Id
    ,school.DistrictId
    ,school.[Key]
    ,school.CommunityId
    ,community.Name as CommunityName
    ,school.Treatment
    ,school.OrganizationId
    ,school.AddressId
    ,(select count(1) from [Education].[SchoolContact-Active] where SchoolId = school.Id) as SchoolContactCount
from SchoolCte cte
    inner join [Education].[School-Active] school on cte.Id = school.Id
    left join [Framework].[UsaPostal-Active] address on school.AddressId = address.Id
    left join [Education].[Community-Active] community on school.CommunityId = community.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)