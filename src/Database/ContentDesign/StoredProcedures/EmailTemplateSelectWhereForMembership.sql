create proc [ContentDesign].[EmailTemplateSelectWhereForMembership] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
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

;with EmailTemplateCte
as (
    select
        row_number() over (order by emailTemplate.Id asc) as RowNumber
        ,count(*) over () as TotalCount
        ,emailTemplate.Id
    from [Content].[EmailTemplate-Active] emailTemplate
        inner join [Content].[Membership-Active] membership on emailTemplate.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where 1 = 1
        and emailTemplate.MembershipId = @MembershipId
        and organization.Id = @organizationId
        and (@SearchText is null
            or 1=1
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,emailTemplate.Id
    ,emailTemplate.MembershipId
    ,emailTemplate.Title
    ,emailTemplate.Subject
    ,emailTemplate.Body
from EmailTemplateCte cte
    inner join [Content].[EmailTemplate-Active] emailTemplate on cte.Id = emailTemplate.Id
    inner join [Content].[Membership-Active] membership on emailTemplate.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)