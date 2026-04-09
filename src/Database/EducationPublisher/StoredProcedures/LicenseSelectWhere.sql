create proc [EducationPublisher].[LicenseSelectWhere] (
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

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with LicenseCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then license.Name
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then license.Name
                end desc,
                case when (@SortAscending = 1)
                    then license.Id
                end asc,
                case when (@SortAscending = 0)
                    then license.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,license.Id
    from [Framework].[License-Active] license
        inner join [Framework].[Organization-Active] organization on license.OwnerId = organization.Id
    where 1 = 1
        and organization.Id = @organizationId
        and (@SearchText is null
            or license.Name like '%' + @SearchText + '%'
            or license.Description like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,license.Id
    ,license.Name
    ,license.Description
    ,(select count(1) from [Education].[DistrictLicense-Active] where LicenseId = license.Id) as DistrictLicenseCount
    ,(select count(1) from [Education].[UnitLicense-Active] where LicenseId = license.Id) as UnitLicenseCount
from LicenseCte cte
    inner join [Framework].[License-Active] license on cte.Id = license.Id
    inner join [Framework].[Organization-Active] organization on license.OwnerId = organization.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)