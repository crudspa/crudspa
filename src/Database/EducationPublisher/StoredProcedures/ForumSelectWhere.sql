create proc [EducationPublisher].[ForumSelectWhere] (
     @PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Districts Framework.IdList readonly
) as

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @districtsCount int = (select count(1) from @Districts)

;with ForumCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then forum.Name
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then forum.Name
                end desc,
                case when (@SortField = 'Pinned' and @SortAscending = 1)
                    then forum.Pinned
                end asc,
                case when (@SortField = 'Pinned' and @SortAscending = 0)
                    then forum.Pinned
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,forum.Id
    from [Education].[Forum-Active] forum
    where 1 = 1
        and (@SearchText is null
            or forum.Name like '%' + @SearchText + '%'
            or forum.Description like '%' + @SearchText + '%'
        )
        and (@districtsCount = 0 or forum.DistrictId in (select Id from @Districts))
)

select
    cte.RowNumber
    ,cte.TotalCount
    ,forum.Id
    ,forum.Name
    ,forum.Description
    ,forum.BodyTemplateId
    ,forum.Pinned
    ,forum.DistrictId
    ,forum.SchoolId
    ,forum.InnovatorsOnly
    ,bodyTemplate.Name as BodyTemplateName
    ,districtOrganization.Name as DistrictName
    ,schoolOrganization.Name as SchoolName
    ,(select count(1) from [Education].[Post-Active] where ForumId = forum.Id) as PostCount
from ForumCte cte
    inner join [Education].[Forum-Active] forum on cte.Id = forum.Id
    inner join [Education].[BodyTemplate-Active] bodyTemplate on forum.BodyTemplateId = bodyTemplate.Id
    left join [Education].[District-Active] district on forum.DistrictId = district.Id
    left join [Framework].[Organization-Active] districtOrganization on district.OrganizationId = districtOrganization.Id
    left join [Education].[School-Active] school on forum.SchoolId = school.Id
    left join [Framework].[Organization-Active] schoolOrganization on school.OrganizationId = schoolOrganization.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc