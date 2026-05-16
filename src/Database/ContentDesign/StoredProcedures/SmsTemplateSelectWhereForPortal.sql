create proc [ContentDesign].[SmsTemplateSelectWhereForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
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

;with SmsTemplateCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Title' and @SortAscending = 1) then smsTemplate.Title end asc,
                case when (@SortField = 'Title' and @SortAscending = 0) then smsTemplate.Title end desc,
                case when (@SortAscending = 1) then smsTemplate.Id end asc,
                case when (@SortAscending = 0) then smsTemplate.Id end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,smsTemplate.Id
    from [Content].[SmsTemplate-Active] smsTemplate
        inner join [Content].[Membership-Active] membership on smsTemplate.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where 1 = 1
        and membership.PortalId = @PortalId
        and organization.Id = @organizationId
        and (@SearchText is null
            or smsTemplate.Title like '%' + @SearchText + '%'
            or smsTemplate.Body like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,smsTemplate.Id
    ,smsTemplate.MembershipId
    ,smsTemplate.Title
    ,smsTemplate.Body
from SmsTemplateCte cte
    inner join [Content].[SmsTemplate-Active] smsTemplate on cte.Id = smsTemplate.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)