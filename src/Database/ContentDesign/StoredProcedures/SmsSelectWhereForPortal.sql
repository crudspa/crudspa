create proc [ContentDesign].[SmsSelectWhereForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@SendStart datetimeoffset(7)
    ,@SendEnd datetimeoffset(7)
    ,@ProcessedStart datetimeoffset(7)
    ,@ProcessedEnd datetimeoffset(7)
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

;with SmsCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Send' and @SortAscending = 1) then sms.Send end asc,
                case when (@SortField = 'Send' and @SortAscending = 0) then sms.Send end desc,
                case when (@SortField = 'Status' and @SortAscending = 1) then sms.Status end asc,
                case when (@SortField = 'Status' and @SortAscending = 0) then sms.Status end desc,
                case when (@SortAscending = 1) then sms.Id end asc,
                case when (@SortAscending = 0) then sms.Id end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,sms.Id
    from [Content].[Sms-Active] sms
        inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where 1 = 1
        and membership.PortalId = @PortalId
        and organization.Id = @organizationId
        and (@SearchText is null
            or sms.Body like '%' + @SearchText + '%'
        )
        and (@SendStart is null or sms.Send >= @SendStart)
        and (@SendEnd is null or sms.Send < @SendEnd)
        and (@ProcessedStart is null or sms.Processed >= @ProcessedStart)
        and (@ProcessedEnd is null or sms.Processed < @ProcessedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,sms.Id
    ,sms.MembershipId
    ,membership.Name as MembershipName
    ,sms.Body
    ,sms.Send
    ,sms.Status
    ,sms.Processed
from SmsCte cte
    inner join [Content].[Sms-Active] sms on cte.Id = sms.Id
    inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)